using MyNoSqlServer.Abstractions;
using Service.Simulation.FTX.Grpc.Models;

namespace Service.Simulation.FTX.NoSql
{
    public class BalancesNoSql: MyNoSqlDbEntity
    {
        public const string TableName = "jetwallet-simulation-ftx-balances";

        public static string GeneratePartitionKey() => "ftx-balances";
        public static string GenerateRowKey(string asset) => asset;

        public GetBalancesResponse.Balance Balance { get; set; }

        public static BalancesNoSql Create(GetBalancesResponse.Balance balance)
        {
            return new BalancesNoSql()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(balance.Symbol),
                Balance = balance
            };
        }
    }
}