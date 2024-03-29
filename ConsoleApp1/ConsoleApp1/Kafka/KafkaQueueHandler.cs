using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace ConsoleApp1
{
    public class KafkaQueueHandler
    {
        public string GetLastMessage(string brokerList, string topicName)
        {
            
            CancellationTokenSource cts = new CancellationTokenSource();
                     
            var config = new ConsumerConfig
            {
                GroupId = "groupid-not-used-but-mandatory",
                BootstrapServers = brokerList,
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Latest
            };
        
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {

                var adminClient = new AdminClientBuilder(config).Build();
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));

                Offset offset = Offset.Beginning;

                var topicMetadata = metadata.Topics.FirstOrDefault(t => t.Topic == topicName);
                if (topicMetadata != null)
                {
                    foreach (var partition in topicMetadata.Partitions)
                    {
                        var topicPartition = new TopicPartition(topicName, new Partition(partition.PartitionId));
                        var offSet = consumer.QueryWatermarkOffsets(topicPartition, TimeSpan.FromSeconds(5));
                        offset = offSet.High;
  
                    }                                   
                }
                                           
                consumer.Assign(new TopicPartitionOffset(topicName, 0, offset - 1));
                var result = consumer.Consume(cts.Token);                                    
                return result.Message.Value;
            }

        }
    }
}
