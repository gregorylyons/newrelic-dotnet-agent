﻿using System;
using System.Data.SqlClient;
using System.Net;
using NewRelic.Agent.IntegrationTestHelpers.RemoteServiceFixtures;
using NewRelic.Agent.IntegrationTests.Shared;
using Xunit;

namespace NewRelic.Agent.UnboundedIntegrationTests.RemoteServiceFixtures
{
    public class MsSqlBasicMvcFixture : RemoteApplicationFixture
    {
        private const String CreatePersonTableMsSql = "CREATE TABLE {0} (FirstName varchar(20) NOT NULL, LastName varchar(20) NOT NULL, Email varchar(50) NOT NULL)";
        private const String DropPersonTableMsSql = "DROP TABLE {0}";
        private const String TargetFramework = "net452";

        private readonly String _connectionString = MsSqlConfiguration.MsSqlConnectionString;

        private readonly String _tableName;
        public String TableName
        {
            get { return _tableName; }
        }


        public MsSqlBasicMvcFixture() : base(new RemoteWebApplication("BasicMvcApplication", ApplicationType.Unbounded))
        {
            _tableName = GenerateTableName();
            CreateTable();
        }

        public void GetMsSql()
        {
            var address = $"http://{DestinationServerName}:{Port}/Default/MsSql?tableName={TableName}";

            using (var webClient = new WebClient())
            {
                var responseBody = webClient.DownloadString(address);
                Assert.NotNull(responseBody);
            }
        }

        public void GetMsSqlAsync()
        {
            var address = $"http://{DestinationServerName}:{Port}/Default/MsSqlAsync?tableName={TableName}";

            using (var webClient = new WebClient())
            {
                var responseBody = webClient.DownloadString(address);
                Assert.NotNull(responseBody);
            }
        }

        public void GetMsSql_WithParameterizedQuery()
        {
            var address = $"http://{DestinationServerName}:{Port}/Default/MsSql_WithParameterizedQuery?tableName={TableName}";

            using (var webClient = new WebClient())
            {
                var responseBody = webClient.DownloadString(address);
                Assert.NotNull(responseBody);
            }
        }

        public void GetMsSqlAsync_WithParameterizedQuery()
        {
            var address = $"http://{DestinationServerName}:{Port}/Default/MsSqlAsync_WithParameterizedQuery?tableName={TableName}";

            using (var webClient = new WebClient())
            {
                var responseBody = webClient.DownloadString(address);
                Assert.NotNull(responseBody);
            }
        }

        public void GetEnterpriseLibraryMsSql()
        {
            var address = $"http://{DestinationServerName}:{Port}/Default/EnterpriseLibraryMsSql?tableName={TableName}";

            using (var webClient = new WebClient())
            {
                var responseBody = webClient.DownloadString(address);
                Assert.NotNull(responseBody);
            }
        }

        private static String GenerateTableName()
        {
            var tableId = Guid.NewGuid().ToString("N").ToLower();
            return $"person{tableId}";
        }

        private void CreateTable()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var createTable = String.Format(CreatePersonTableMsSql, TableName);
                using (var command = new SqlCommand(createTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DropTable()
        {
            var dropTableSql = String.Format(DropPersonTableMsSql, TableName);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(dropTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            DropTable();
        }
    }
}
