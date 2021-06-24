using System.ServiceModel;
using System.Threading.Tasks;
using Service.Simulation.Grpc.Models;

namespace Service.Simulation.Grpc
{
    [ServiceContract]
    public interface ISimulationTradingService
    {
        [OperationContract]
        Task<ExecuteMarketOrderResponse> ExecuteMarketOrderAsync(ExecuteMarketOrderRequest request);

        [OperationContract]
        Task<GetBalancesResponse> GetBalancesAsync();

        [OperationContract]
        Task<GetMarketInfoResponse> GetMarketInfoAsync(GetMarketInfoRequest request);

        [OperationContract]
        Task<GetMarketInfoListResponse> GetMarketInfoListAsync();

        [OperationContract]
        Task SetBalanceAsync(SetBalanceRequest request);
    }
}