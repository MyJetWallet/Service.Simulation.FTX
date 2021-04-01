using System.Runtime.Serialization;

namespace Service.Simulation.FTX.Grpc.Models
{
    [DataContract]
    public class SetBalanceRequest
    {
        [DataMember(Order = 1)] public string Symbol { get; set; }
        [DataMember(Order = 2)] public double Amount { get; set; }
    }
}