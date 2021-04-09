﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.ExternalMarketApi;
using MyJetWallet.Domain.ExternalMarketApi.Dto;
using MyJetWallet.Domain.ExternalMarketApi.Models;
using MyJetWallet.Domain.Orders;
using MyJetWallet.Sdk.Service;
using Newtonsoft.Json;
using Service.Simulation.FTX.Grpc;
using Service.Simulation.FTX.Grpc.Models;
using GetBalancesResponse = MyJetWallet.Domain.ExternalMarketApi.Dto.GetBalancesResponse;
using GetMarketInfoListResponse = MyJetWallet.Domain.ExternalMarketApi.Dto.GetMarketInfoListResponse;
using GetMarketInfoResponse = MyJetWallet.Domain.ExternalMarketApi.Dto.GetMarketInfoResponse;

namespace Service.Simulation.FTX.Services
{
    public class ExternalMarketGrpc: IExternalMarket
    {
        private static Dictionary<string, ExchangeMarketInfo> _marketInfoData = new Dictionary<string, ExchangeMarketInfo>();

        private readonly ILogger<ExternalMarketGrpc> _logger;
        private readonly ISimulationFtxTradingService _service;

        public ExternalMarketGrpc(
            ILogger<ExternalMarketGrpc> logger,
            ISimulationFtxTradingService service)
        {
            _logger = logger;
            _service = service;
        }

        public Task<GetNameResult> GetNameAsync()
        {
            return Task.FromResult(new GetNameResult() {Name = "Simulation-FTX"});
        }

        public async Task<GetBalancesResponse> GetBalancesAsync()
        {
            var resp = await _service.GetBalancesAsync();
            var result = resp.Balances.ToDictionary(e => e.Symbol, e => e.Amount);
            return new GetBalancesResponse(){Balances = result};
        }

        public async Task<GetMarketInfoResponse> GetMarketInfoAsync(MarketRequest request)
        {
            if (_marketInfoData!.Any() != true)
                await LoadMarketInfo();

            if (_marketInfoData.TryGetValue(request.Market, out var resp))
                return new GetMarketInfoResponse(){Info = resp};

            return new GetMarketInfoResponse() { Info = null };
        }

        public async Task<GetMarketInfoListResponse> GetMarketInfoListAsync()
        {
            if (_marketInfoData!.Any() != true)
                await LoadMarketInfo();

            return new GetMarketInfoListResponse() {Infos = _marketInfoData.Values.ToList()};
        }

        public async Task<ExchangeTrade> MarketTrade(string market, OrderSide side, double volume, string referenceId)
        {
            using var activity = MyTelemetry.StartActivity("MarketTrade");

            try
            {
                var request = new ExecuteMarketOrderRequest()
                {
                    ClientId = referenceId,
                    Market = market,
                    Side = side == OrderSide.Buy ? SimulationFtxOrderSide.Buy : SimulationFtxOrderSide.Sell,
                    Size = volume
                };

                request.AddToActivityAsJsonTag("request");

                var marketInfo = await GetMarketInfoAsync(new MarketRequest() {Market = market});
                if (marketInfo?.Info == null)
                {
                    throw new Exception(
                        $"Cannot execute trade, market info do not found. Request: {JsonConvert.SerializeObject(request)}");
                }

                marketInfo.AddToActivityAsJsonTag("market-info");

                var resp = await _service.ExecuteMarketOrderAsync(request);

                resp.AddToActivityAsJsonTag("response");

                if (!resp.Success)
                {
                    throw new Exception(
                        "Cannot execute trade in simulation ftx. Request: {JsonConvert.SerializeObject(request)}");
                }

                var result = new ExchangeTrade()
                {
                    Id = resp.Trade.Id,
                    Market = resp.Trade.Market,
                    Price = resp.Trade.Price,
                    Volume = resp.Trade.Size,
                    Timestamp = resp.Trade.Timestamp,
                    Side = resp.Trade.Side == SimulationFtxOrderSide.Buy ? OrderSide.Buy : OrderSide.Sell,
                    OppositeVolume = (double) ((decimal) resp.Trade.Price * (decimal) resp.Trade.Size),
                    Source = (await GetNameAsync()).Name
                };

                result.AddToActivityAsJsonTag("result");

                return result;
            }
            catch(Exception ex)
            {
                ex.FailActivity();
                _logger.LogError(ex, "Cannot execute MarketTrade in simulation FTX");
                throw;
            }
        }

        private async Task LoadMarketInfo()
        {
            using var activity = MyTelemetry.StartActivity("Load market info");
            try
            {
                var data = await _service.GetMarketInfoListAsync();

                var result = new Dictionary<string, ExchangeMarketInfo>();

                foreach (var marketInfo in data.Info)
                {
                    var resp = new ExchangeMarketInfo()
                    {
                        Market = marketInfo.Market,
                        MinVolume = marketInfo.MinVolume,
                        PriceAccuracy = marketInfo.PriceAccuracy,
                        BaseAsset = marketInfo.BaseAsset,
                        QuoteAsset = marketInfo.QuoteAsset,
                        VolumeAccuracy = marketInfo.BaseAccuracy
                    };

                    result[resp.Market] = resp;
                }

                _marketInfoData = result;
            }
            catch (Exception ex)
            {
                ex.FailActivity();
                throw;
            }
        }
    }
}