using System.Security.Cryptography;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyNoSqlServer.Abstractions;
using Service.Simulation.FTX.Grpc;
using Service.Simulation.FTX.NoSql;
using Service.Simulation.FTX.Services;

namespace Service.Simulation.FTX.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<FTXDataApi>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<TradeHistory>()
                .AsSelf()
                .SingleInstance();

            RegisterMyNoSqlWriter<BalancesNoSql>(builder, BalancesNoSql.TableName);

            builder
                .RegisterType<SimulationTradingService>()
                .As<ISimulationTradingService>();

            builder
                .RegisterType<OrderBookManager>()
                .AsSelf()
                .SingleInstance();

        }

        private void RegisterMyNoSqlWriter<TEntity>(ContainerBuilder builder, string table)
            where TEntity : IMyNoSqlDbEntity, new()
        {
            builder.Register(ctx => new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<TEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), table, true))
                .As<IMyNoSqlServerDataWriter<TEntity>>()
                .SingleInstance();

        }
    }
}