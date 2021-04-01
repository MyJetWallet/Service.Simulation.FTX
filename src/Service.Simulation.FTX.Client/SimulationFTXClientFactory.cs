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
    public class SimulationFTXClientFactory
    {
        private readonly CallInvoker _channel;

        public SimulationFTXClientFactory(string assetsDictionaryGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(assetsDictionaryGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public ISimulationFtxTradingService GetSimulationFtxTradingService() => _channel.CreateGrpcService<ISimulationFtxTradingService>();
        public ISimulationFtxTradeHistoryService GetSimulationFtxTradeHistoryService() => _channel.CreateGrpcService<ISimulationFtxTradeHistoryService>();

        
    }
}
