using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.Simulation.FTX.Client;
using Service.Simulation.FTX.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new SimulationFTXClientFactory("http://localhost:80");
            var client = factory.GetSimulationFtxTradingService();

            

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
