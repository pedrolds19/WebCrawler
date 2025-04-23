using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Home
{
    public class HomeDAL
    {
        private readonly string _connectionString;

        public HomeDAL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task GravarLog(CrawlerLogs log)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "SPI_GRAVAR_LOG",
                new
                {
                    StartTime = log.StartTime,
                    EndTime = log.EndTime,
                    TotalPages = log.TotalPages,
                    TotalRows = log.TotalRows,
                    JsonPath = log.JsonPath
                },
                commandType: CommandType.StoredProcedure
            );
        }


    }
}
