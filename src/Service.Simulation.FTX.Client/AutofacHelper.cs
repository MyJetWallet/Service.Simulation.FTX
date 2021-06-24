using Autofac;
using Service.Simulation.FTX.Grpc;

namespace Service.Simulation.FTX.Client
{
    public static class AutofacHelper
    {
        public static void RegisterSimulationFtxClient(this ContainerBuilder builder, string simulationFtxGrpcServiceUrl)
        {
            var factory = new SimulationClientFactory(simulationFtxGrpcServiceUrl);

            builder.RegisterInstance(factory.GetSimulationFtxTradingService()).As<ISimulationTradingService>().SingleInstance();
            builder.RegisterInstance(factory.GetSimulationFtxTradeHistoryService()).As<ISimulationTradeHistoryService>().SingleInstance();
        }
    }
}