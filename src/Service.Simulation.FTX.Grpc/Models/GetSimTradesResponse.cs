using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.Simulation.Grpc.Models
{
    [DataContract]
    public class GetSimTradesResponse
    {
        [DataMember(Order = 1)] public List<SimTrade> Trades { get; set; }
    }
}