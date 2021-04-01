using System.Runtime.Serialization;

namespace Service.Simulation.FTX.Grpc.Models
{
    [DataContract]
    public class ExecuteMarketOrderResponse
    {
        [DataMember(Order = 1)] public bool Success { get; set; }

        [DataMember(Order = 2)] public FtxSimTrade Trade { get; set; }
    }
}