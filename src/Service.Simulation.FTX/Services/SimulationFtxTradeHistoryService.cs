using System.Threading.Tasks;
using Service.Simulation.FTX.Grpc;
using Service.Simulation.FTX.Grpc.Models;

namespace Service.Simulation.FTX.Services
{
    public class SimulationTradeHistoryService: ISimulationTradeHistoryService
    {
        private readonly TradeHistory _history;

        public SimulationTradeHistoryService(TradeHistory history)
        {
            _history = history;
        }

        public Task<GetFtxSimTradesResponse> GetTradesAsync()
        {
            var data = _history.GetTrades();
            var resp = new GetFtxSimTradesResponse()
            {
                Trades = data
            };

            return Task.FromResult(resp);
        }
    }
}