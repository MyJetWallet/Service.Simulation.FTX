using System.Runtime.Serialization;

namespace Service.Simulation.Grpc.Models
{
    [DataContract]
    public class GetMarketInfoRequest
    {
        [DataMember(Order = 1)] public string Market { get; set; }
    }
}