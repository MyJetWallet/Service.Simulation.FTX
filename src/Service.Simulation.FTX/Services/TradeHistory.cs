using System.Collections.Generic;
using System.Linq;
using Service.Simulation.FTX.Grpc.Models;

namespace Service.Simulation.FTX.Services
{
    public class TradeHistory
    {
        private List<FtxSimTrade> _data = new List<FtxSimTrade>();

        public void AddTrade(FtxSimTrade trade)
        {
            lock (_data)
            {
                _data.Insert(0, trade);

                while (_data.Count > 50)
                {
                    _data.RemoveAt(_data.Count - 1);
                }
            }
        }

        public List<FtxSimTrade> GetTrades()
        {
            lock (_data)
            {
                return _data.ToList();
            }
        }

        
    }
}