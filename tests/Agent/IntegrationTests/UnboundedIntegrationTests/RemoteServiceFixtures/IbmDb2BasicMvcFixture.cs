﻿using System;
using System.Net;
using IBM.Data.DB2;
using NewRelic.Agent.IntegrationTestHelpers.RemoteServiceFixtures;
using NewRelic.Agent.IntegrationTests.Shared;
using Xunit;

namespace NewRelic.Agent.UnboundedIntegrationTests.RemoteServiceFixtures
{
    public class IbmDb2BasicMvcFixture : RemoteApplicationFixture
    {
        private const String CreateHotelTableDB2Sql = "CREATE TABLE {0} (HOTEL_ID INT NOT NULL, BOOKING_DATE DATE NOT NULL, " +
                                                         "ROOMS_TAKEN INT DEFAULT 0, PRIMARY KEY (HOTEL_ID, BOOKING_DATE))";
        private const String DropHotelTableDB2Sql = "DROP TABLE {0}";

        private readonly String _tableName;
        public String TableName
        {
            get { return _tableName; }
        }

        public IbmDb2BasicMvcFixture() : base(new RemoteWebApplication("BasicMvcApplication", ApplicationType.Unbounded))
        {
            _tableName = GenerateTableName();
            CreateTable();
        }

        public void GetIbmDb2()
        {
            var address = $"http://{DestinationServerName}:{Port}/Default/InvokeIbmDb2Query?tableName={TableName}";

            using (var webClient = new WebClient())
            {
                var responseBody = webClient.DownloadString(address);
                Assert.NotNull(responseBody);
            }
        }

        public void GetIbmDb2Async()
        {
            var address = $"http://{DestinationServerName}:{Port}/Default/InvokeIbmDb2QueryAsync?tableName={TableName}";

            using (var webClient = new WebClient())
            {
                var responseBody = webClient.DownloadString(address);
                Assert.NotNull(responseBody);
            }
        }

        private static String GenerateTableName()
        {
            //Oracle tables must start w/ character and be <= 30 length. Table name = H{tableId}
            var tableId = Guid.NewGuid().ToString("N").Substring(2, 29).ToLower();
            return $"h{tableId}";
        }

        private void CreateTable()
        {
            var createTable = String.Format(CreateHotelTableDB2Sql, TableName);
            using (var connection = new DB2Connection(Db2Configuration.Db2ConnectionString))
            {
                connection.Open();

                using (var command = new DB2Command(createTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DropTable()
        {
            var dropTableSql = String.Format(DropHotelTableDB2Sql, TableName);

            using (var connection = new DB2Connection(Db2Configuration.Db2ConnectionString))
            {
                connection.Open();

                using (var command = new DB2Command(dropTableSql, connection))
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
