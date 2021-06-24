using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.Simulation.FTX.Client;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new SimulationFtxClientFactory("http://simulation-ftx-2.services.svc.cluster.local:80");
            var client = factory.GetSimulationFtxTradingService();

            

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
