using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class KafkaProducer
    {
        public async Task SendMessage(string message, string brokerList, string topicName)
        {
           
            var config = new ProducerConfig { BootstrapServers = brokerList };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                var cancelled = false;
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cancelled = true;
                };

                while (!cancelled)
                {
                    
                    string key = null;
                    string val = message;
                     
                    try
                    {                       
                        var deliveryReport = await producer.ProduceAsync(
                            topicName, new Message<string, string> { Key = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), Value = val });
                        
                        cancelled = true;

                        Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                    }
                    catch (ProduceException<string, string> e)
                    {
                        Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    }

                    await Task.Delay(1000);
                }
            }

        } 
    }
}
