using Consumer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;


IConfiguration configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json")
   .Build();


var services = new ServiceCollection()
          .AddLogging()
          .AddSingleton<ClickhouseHandler>()           
          .AddSingleton<IConfiguration>(configuration)
          .BuildServiceProvider();



CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => {
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

 

KafkaConsumer.Run_Consume(
     configuration.GetSection("Settings:Kafka:broker").Value,
     new List<string>() { configuration.GetSection("Settings:Kafka:topic").Value },
     cts.Token,
     Send
     );


void Send(string message, DateTime time)
{
    try
    {
        var clickHouseHandler = services.GetService<ClickhouseHandler>();
        Message msg = JsonSerializer.Deserialize<Message>(message);
        clickHouseHandler.Save(msg, time);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }    
}

