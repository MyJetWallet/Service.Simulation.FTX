using System.Runtime.Serialization;

namespace Service.Simulation.FTX.Grpc.Models
{
    [DataContract]
    public class GetMarketInfoResponse
    {
        [DataMember(Order = 1)] public MarketInfo Info { get; set; }

        [DataContract]
        public class MarketInfo
        {
            [DataMember(Order = 1)] public string Market { get; set; }
            [DataMember(Order = 2)] public string BaseAsset { get; set; }
            [DataMember(Order = 3)] public string QuoteAsset { get; set; }
        }
    }
}