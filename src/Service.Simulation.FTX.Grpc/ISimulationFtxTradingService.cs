using System.ServiceModel;
using System.Threading.Tasks;
using Service.Simulation.FTX.Grpc.Models;

namespace Service.Simulation.FTX.Grpc
{
    [ServiceContract]
    public interface ISimulationFtxTradingService
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