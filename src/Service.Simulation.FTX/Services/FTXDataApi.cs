using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Service.Simulation.FTX.Services
{
    public class FTXDataApi
    {
        private readonly HttpClient _client = new HttpClient();

        public async Task<FTXOrderBook> GetOrderBook(string market)
        {
            var json = await _client.GetStringAsync($"https://ftx.com/api/markets/{market}/orderbook?depth=100");
            var resp = JsonConvert.DeserializeObject<FTXOrderBook>(json);
            return resp;
        }

        public async Task<FTXMarket> GetMarketInfo(string market)
        {
            var json = await _client.GetStringAsync($"https://ftx.com/api/markets/{market}");
            var resp = JsonConvert.DeserializeObject<FTXMarket>(json);
            return resp;
        }
    }

    public class FTXOrderBook
    {
        public bool success { get; set; }
        public string error { get; set; }

        public ResultData result { get; set; }

        public class ResultData
        {
            public double[][] bids { get; set; }
            public double[][] asks { get; set; }
        }
    }

    public class FTXMarket
    {
        public bool success { get; set; }
        public string error { get; set; }

        public ResultData result { get; set; }

        public class ResultData
        {
            public string name { get; set; }
            public string baseCurrency { get; set; }
            public string quoteCurrency { get; set; }
        }
    }
}