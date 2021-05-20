using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Service.Simulation.FTX.Grpc;
using Service.Simulation.FTX.Grpc.Models;
using Service.Simulation.FTX.NoSql;

namespace Service.Simulation.FTX.Services
{
    internal class SimulationFtxTradingService : ISimulationFtxTradingService
    {
        private readonly ILogger<SimulationFtxTradingService> _logger;
        private readonly FTXDataApi _api;
        private readonly IMyNoSqlServerDataWriter<BalancesNoSql> _balanceWriter;
        private readonly TradeHistory _history;

        public SimulationFtxTradingService(ILogger<SimulationFtxTradingService> logger, FTXDataApi api, IMyNoSqlServerDataWriter<BalancesNoSql> balanceWriter,
            TradeHistory history)
        {
            _logger = logger;
            _api = api;
            _balanceWriter = balanceWriter;
            _history = history;
        }

        public async Task<ExecuteMarketOrderResponse> ExecuteMarketOrderAsync(ExecuteMarketOrderRequest request)
        {
            _logger.LogInformation("ExecuteMarketOrderAsync Request: {tradeText}", JsonConvert.SerializeObject(request));

            var marketResp = await GetMarketInfoAsync(new GetMarketInfoRequest() {Market = request.Market});

            if (marketResp.Info == null)
            {
                _logger.LogError("Cannot execute market order: {jsonText}", JsonConvert.SerializeObject(request));
                return new ExecuteMarketOrderResponse()
                {
                    Success = false,
                };
            }

            var market = marketResp.Info;

            var orderBookResp = await _api.GetOrderBook(request.Market);
            if (!orderBookResp.success)
            {
                _logger.LogError("Cannot execute market order, order book do not found: {errorText}. Request: {jsonText}", orderBookResp.error, JsonConvert.SerializeObject(request));
                return new ExecuteMarketOrderResponse()
                {
                    Success = false,
                };
            }

            var balances = (await GetBalancesAsync()).Balances;

            var baseBalance = balances.FirstOrDefault(e => e.Symbol == market.BaseAsset);
            var quoteBalance = balances.FirstOrDefault(e => e.Symbol == market.QuoteAsset);

            if (request.Side == SimulationFtxOrderSide.Buy)
            {
                var remindVolume = request.Size;
                var quoteVolume = 0.0;

                var levels = orderBookResp.result.asks.OrderBy(e => e[0]);
                foreach (var level in levels)
                {
                    if (level[1] >= remindVolume)
                    {
                        quoteVolume += remindVolume * level[0];
                        remindVolume = 0;
                        break;
                    }

                    quoteVolume += level[1] * level[0];
                    remindVolume -= level[1];
                }

                if (remindVolume != 0)
                {
                    _logger.LogError(
                        "Cannot execute market order, not enough liquidity. RemindVolume: {remindNumber}. Request: {jsonText}",
                        remindVolume, JsonConvert.SerializeObject(request));
                    return new ExecuteMarketOrderResponse()
                    {
                        Success = false,
                    };
                }


                if (quoteBalance == null || quoteBalance.Amount < quoteVolume)
                {
                    _logger.LogError(
                        "Cannot execute market order, not enough balance. Required: {requiredNumber}; Exist: {existNumber}. Request: {jsonText}",
                        quoteBalance, quoteBalance?.Amount ?? 0.0, orderBookResp.error,
                        JsonConvert.SerializeObject(request));
                    return new ExecuteMarketOrderResponse()
                    {
                        Success = false,
                        
                    };
                }

                quoteBalance.Amount -= quoteVolume;
                if (baseBalance == null) baseBalance = new GetBalancesResponse.Balance() { Symbol = market.BaseAsset, Amount = 0 };
                baseBalance.Amount += request.Size;

                await SetBalanceAsync(new SetBalanceRequest() {Symbol = quoteBalance.Symbol, Amount = quoteBalance.Amount});
                await SetBalanceAsync(new SetBalanceRequest() { Symbol = baseBalance.Symbol, Amount = baseBalance.Amount });

                var price = quoteVolume / request.Size;

                var trade = new FtxSimTrade()
                {
                    Market = request.Market,
                    ClientId = request.ClientId,
                    Id = Guid.NewGuid().ToString("N"),
                    Side = request.Side,
                    Size = request.Size,
                    Price = price,
                    Timestamp = DateTime.UtcNow
                };

                _history.AddTrade(trade);

                var resp = new ExecuteMarketOrderResponse()
                {
                    Success = true,
                    Trade = trade
                };

                _logger.LogInformation("Trade: {tradeText}", JsonConvert.SerializeObject(resp));

                return resp;
            }
            else
            {
                var remindVolume = request.Size;
                var quoteVolume = 0.0;

                var levels = orderBookResp.result.bids.OrderByDescending(e => e[0]);
                foreach (var level in levels)
                {
                    if (level[1] >= remindVolume)
                    {
                        quoteVolume += remindVolume * level[0];
                        remindVolume = 0;
                        break;
                    }

                    quoteVolume += level[1] * level[0];
                    remindVolume -= level[1];
                }

                if (remindVolume != 0)
                {
                    _logger.LogError(
                        "Cannot execute market order, not enough liquidity. RemindVolume: {remindNumber}. Request: {jsonText}",
                        remindVolume, JsonConvert.SerializeObject(request));
                    return new ExecuteMarketOrderResponse()
                    {
                        Success = false,
                    };
                }


                if (baseBalance == null || baseBalance.Amount < request.Size)
                {
                    _logger.LogError(
                        "Cannot execute market order, not enough balance. Required: {requiredNumber}; Exist: {existNumber}. Request: {jsonText}",
                        request.Side, baseBalance?.Amount ?? 0.0, orderBookResp.error,
                        JsonConvert.SerializeObject(request));
                    return new ExecuteMarketOrderResponse()
                    {
                        Success = false,
                    };
                }

                baseBalance.Amount -= request.Size;

                if (quoteBalance == null) quoteBalance = new GetBalancesResponse.Balance() { Symbol = market.QuoteAsset, Amount = 0};
                quoteBalance.Amount += quoteVolume;

                await SetBalanceAsync(new SetBalanceRequest() { Symbol = baseBalance.Symbol, Amount = baseBalance.Amount });
                await SetBalanceAsync(new SetBalanceRequest() { Symbol = quoteBalance.Symbol, Amount = quoteBalance.Amount });

                var price = quoteVolume / request.Size;

                var trade = new FtxSimTrade()
                {
                    Market = request.Market,
                    ClientId = request.ClientId,
                    Id = Guid.NewGuid().ToString("N"),
                    Side = request.Side,
                    Size = request.Size,
                    Price = price,
                    Timestamp = DateTime.UtcNow
                };

                _history.AddTrade(trade);

                var resp = new ExecuteMarketOrderResponse()
                {
                    Success = true,
                    Trade = trade
                };

                _logger.LogInformation("Trade: {tradeText}", JsonConvert.SerializeObject(resp));

                return resp;
            }
        }

        public async Task<GetBalancesResponse> GetBalancesAsync()
        {
            var data = await _balanceWriter.GetAsync(BalancesNoSql.GeneratePartitionKey(Program.Settings.Name));
            return new GetBalancesResponse()
            {
                Balances = data.Select(e => e.Balance).ToList()
            };
        }

        public async Task<GetMarketInfoResponse> GetMarketInfoAsync(GetMarketInfoRequest request)
        {
            var data = await _api.GetMarketInfo(request.Market);

            if (!data.success)
            {
                _logger.LogError("Cannot found market {market}, error: {errorText}", request.Market, data.error);
                return new GetMarketInfoResponse();
            }

            var resp = new GetMarketInfoResponse()
            {
                Info = ReadMarket(data.result)
            };

            return resp;
        }

        public async Task<GetMarketInfoListResponse> GetMarketInfoListAsync()
        {
            var data = await _api.GetMarketInfoList();

            var resp = new GetMarketInfoListResponse()
            {
                Info = new List<MarketInfo>()
            };

            foreach (var market in data.result.Where(e => e.type == "spot" && e.enabled))
            {
                resp.Info.Add(ReadMarket(market));
            }

            return resp;
        }

        private MarketInfo ReadMarket(ResultData market)
        {
            var result = new MarketInfo()
            {
                Market = market.name,
                BaseAsset = market.baseCurrency,
                QuoteAsset = market.quoteCurrency,
                MinVolume = (double)market.minProvideSize
            };

            //var volumeParams = market.sizeIncrement.ToString(CultureInfo.InvariantCulture).Split('.');
            //var priceParams = market.priceIncrement.ToString(CultureInfo.InvariantCulture).Split('.');
            result.BaseAccuracy = 8; //volumeParams.Length == 2 ? volumeParams.Length : 0;
            result.PriceAccuracy = 8; // priceParams.Length == 2 ? priceParams.Length : 0;

            return result;
        }

        public async Task SetBalanceAsync(SetBalanceRequest request)
        {
            var item = BalancesNoSql.Create(Program.Settings.Name, new GetBalancesResponse.Balance()
            {
                Symbol = request.Symbol,
                Amount = request.Amount
            });

            await _balanceWriter.InsertOrReplaceAsync(item);
        }
    }

    
}