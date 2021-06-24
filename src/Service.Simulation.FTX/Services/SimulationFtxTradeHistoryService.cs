﻿using System.Threading.Tasks;
using Service.Simulation.Grpc;
using Service.Simulation.Grpc.Models;

namespace Service.Simulation.FTX.Services
{
    public class SimulationTradeHistoryService : ISimulationTradeHistoryService
    {
        private readonly TradeHistory _history;

        public SimulationTradeHistoryService(TradeHistory history)
        {
            _history = history;
        }

        public Task<GetSimTradesResponse> GetTradesAsync()
        {
            var data = _history.GetTrades();
            var resp = new GetSimTradesResponse()
            {
                Trades = data
            };

            return Task.FromResult(resp);
        }
    }
}