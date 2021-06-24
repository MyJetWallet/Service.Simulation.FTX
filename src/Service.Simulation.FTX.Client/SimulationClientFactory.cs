using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.GrpcMetrics;
using ProtoBuf.Grpc.Client;
using Service.Simulation.FTX.Grpc;

namespace Service.Simulation.FTX.Client
{
    [UsedImplicitly]
    public class SimulationClientFactory
    {
        private readonly CallInvoker _channel;

        public SimulationClientFactory(string assetsDictionaryGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(assetsDictionaryGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public ISimulationTradingService GetSimulationFtxTradingService() => _channel.CreateGrpcService<ISimulationTradingService>();
        public ISimulationTradeHistoryService GetSimulationFtxTradeHistoryService() => _channel.CreateGrpcService<ISimulationTradeHistoryService>();

        
    }
}
