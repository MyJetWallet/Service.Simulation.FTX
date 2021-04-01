using System.ServiceModel;
using System.Threading.Tasks;
using Service.Simulation.FTX.Grpc.Models;

namespace Service.Simulation.FTX.Grpc
{
    [ServiceContract]
    public interface ISimulationFtxTradeHistoryService
    {
        [OperationContract]
        Task<GetFtxSimTradesResponse> GetTradesAsync();
    }
}