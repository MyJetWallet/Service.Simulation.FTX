using System.ServiceModel;
using System.Threading.Tasks;
using Service.Simulation.Grpc.Models;

namespace Service.Simulation.Grpc
{
    [ServiceContract]
    public interface ISimulationTradeHistoryService
    {
        [OperationContract]
        Task<GetSimTradesResponse> GetTradesAsync();
    }
}