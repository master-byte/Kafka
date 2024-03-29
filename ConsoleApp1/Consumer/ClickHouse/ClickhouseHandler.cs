using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Octonica.ClickHouseClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    public class ClickhouseHandler
    {
        IConfiguration _configuration;

        public ClickhouseHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Save(Message msg, DateTime dt )
        {
            try
            {                           
                var sb = new ClickHouseConnectionStringBuilder();
                sb.Host = _configuration.GetSection("Settings:ClickHouseSettings:Host").Value;
                sb.Port = Convert.ToUInt16(_configuration.GetSection("Settings:ClickHouseSettings:Port").Value);
                sb.Password = _configuration.GetSection("Settings:ClickHouseSettings:Password").Value;
                sb.User = _configuration.GetSection("Settings:ClickHouseSettings:User").Value;
                sb.Database = _configuration.GetSection("Settings:ClickHouseSettings:Database").Value;


                using var conn = new ClickHouseConnection(sb);
                conn.Open();
               
                using var cmd = conn.CreateCommand("INSERT INTO primes(`number`,nick_name,date_number,date_queue) SELECT   {number},{nick_name}, {date_number},{date_queue}");             
                cmd.Parameters.AddWithValue("number", msg.Number);
                cmd.Parameters.AddWithValue("nick_name", msg.NickName);               
                cmd.Parameters.AddWithValue("date_number", msg.NumberCreated , System.Data.DbType.DateTime);
                cmd.Parameters.AddWithValue("date_queue", dt, System.Data.DbType.DateTime);
                var _ = await cmd.ExecuteNonQueryAsync();
                conn.Close();
                                           
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return;
        }
    }
}