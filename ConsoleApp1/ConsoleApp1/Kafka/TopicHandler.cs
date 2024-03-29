using Confluent.Kafka.Admin;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class TopicHandler
    {

        public async Task CreateTopicAsync(string bootstrapServers, string topicName)
        {
            if (IsExistsTopic(bootstrapServers, topicName))
            {
                return;
            }
                             
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                        new TopicSpecification { Name = topicName, ReplicationFactor = 1, NumPartitions = 1 } });             
                }
                catch (CreateTopicsException e)
                {
                    Console.WriteLine($"An error occurred creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                }
            }                    
        }


        public bool IsExistsTopic(string bootstrapServers, string topicName)
        {
            var adminConfig = new AdminClientConfig()
            {
                BootstrapServers = bootstrapServers
            };

            using (var adminClient = new AdminClientBuilder(adminConfig).Build())
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                var topicsMetadata = metadata.Topics;
                var topicNames = metadata.Topics.Select(a => a.Topic).ToList();

                if (topicNames.Where(x => x.Contains(topicName)).Any())
                {
                    return true;
                }
            }

            return false;
        }

    }
}
