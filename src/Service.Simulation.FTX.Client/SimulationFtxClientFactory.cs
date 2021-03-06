using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.GrpcMetrics;
using ProtoBuf.Grpc.Client;
using Service.Simulation.Grpc;

namespace Service.Simulation.FTX.Client
{
    [UsedImplicitly]
    public class SimulationFtxClientFactory
    {
        private readonly CallInvoker _channel;

        public SimulationFtxClientFactory(string assetsDictionaryGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(assetsDictionaryGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public ISimulationTradingService GetSimulationFtxTradingService() =>
            _channel.CreateGrpcService<ISimulationTradingService>();

        public ISimulationTradeHistoryService GetSimulationFtxTradeHistoryService() =>
            _channel.CreateGrpcService<ISimulationTradeHistoryService>();
    }
}