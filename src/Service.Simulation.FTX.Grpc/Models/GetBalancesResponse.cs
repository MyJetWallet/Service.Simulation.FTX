using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.Simulation.FTX.Grpc.Models
{
    [DataContract]
    public class GetBalancesResponse
    {
        [DataMember(Order = 1)] public List<Balance> Balances { get; set; }

        [DataContract]
        public class Balance
        {
            [DataMember(Order = 1)] public string Symbol { get; set; }
            [DataMember(Order = 2)] public double Amount { get; set; }
        }
    }
}