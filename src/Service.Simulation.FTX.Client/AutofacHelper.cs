using Autofac;
using Service.Simulation.FTX.Grpc;

namespace Service.Simulation.FTX.Client
{
    public static class AutofacHelper
    {
        public static void RegisterLiquidityEngineClient(this ContainerBuilder builder, string simulationFtxGrpcServiceUrl)
        {
            var factory = new SimulationFTXClientFactory(simulationFtxGrpcServiceUrl);

            builder.RegisterInstance(factory.GetSimulationFtxTradingService()).As<ISimulationFtxTradingService>().SingleInstance();
            builder.RegisterInstance(factory.GetSimulationFtxTradeHistoryService()).As<ISimulationFtxTradeHistoryService>().SingleInstance();

            
        }
    }
}