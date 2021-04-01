using System.Runtime.Serialization;

namespace Service.Simulation.FTX.Grpc.Models
{
    [DataContract]
    public class ExecuteMarketOrderRequest
    {
        [DataMember(Order = 1)] public string Market { get; set; }
        [DataMember(Order = 2)] public SimulationFtxOrderSide Side { get; set; }
        [DataMember(Order = 3)] public double Size { get; set; }
        [DataMember(Order = 4)] public string ClientId { get; set; }
    }
}