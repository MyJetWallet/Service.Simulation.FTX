using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.Simulation.FTX.Grpc.Models
{
    [DataContract]
    public class GetFtxSimTradesResponse
    {
        [DataMember(Order = 1)] public List<FtxSimTrade> Trades { get; set; }
    }
}