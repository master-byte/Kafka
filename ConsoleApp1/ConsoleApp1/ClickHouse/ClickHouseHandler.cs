using Microsoft.Extensions.Configuration;
using Octonica.ClickHouseClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1.ClicHouse
{
    public class ClickHouseHandler
    {
        IConfiguration _configuration;

        public ClickHouseHandler(IConfiguration configuration) {
            _configuration = configuration;
        }


        public async Task AddTableAsync()
        {
            try
            {
                //  string  text =  await File.ReadAllTextAsync("/app/init_database.sql");

                  string text = " CREATE TABLE IF NOT EXISTS alfn.primes\r\n(\r\n    id  UInt32 NOT NULL,    \r\n    number  UInt32 NOT NULL,\r\n    nick_name  String NOT NULL, \r\n    date_number DateTime(),\r\n    date_queue DateTime()\r\n)\r\nENGINE = MergeTree()\r\nPRIMARY KEY (id);";

                var sb = new ClickHouseConnectionStringBuilder();
                sb.Host = _configuration.GetSection("Settings:ClickHouseSettings:Host").Value;
                sb.Port = Convert.ToUInt16(_configuration.GetSection("Settings:ClickHouseSettings:Port").Value);
                sb.Password = _configuration.GetSection("Settings:ClickHouseSettings:Password").Value;
                sb.User = _configuration.GetSection("Settings:ClickHouseSettings:User").Value;
                sb.Database = _configuration.GetSection("Settings:ClickHouseSettings:Database").Value;

                using var conn = new ClickHouseConnection(sb);
                await conn.OpenAsync();

                await conn.CreateCommand(text).ExecuteNonQueryAsync();
            }
            catch (Exception ex) 
            { 
              Console.WriteLine(ex.Message);
            }
                       
        }

    }
}
