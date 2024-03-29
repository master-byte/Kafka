using ConsoleApp1;
using ConsoleApp1.ClicHouse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;


IConfiguration configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json" )
   .Build();


var services = new ServiceCollection()
          .AddLogging()
          .AddSingleton<ClickHouseHandler>()
          .AddTransient<TopicHandler>()
          .AddTransient<KafkaQueueHandler>()
          .AddSingleton<IConfiguration>(configuration)
          .BuildServiceProvider();



// Создание таблицы
var clickHouseHandler = services.GetService<ClickHouseHandler>();
await clickHouseHandler.AddTableAsync();


var topicHandler = services.GetService<TopicHandler>();
await topicHandler.CreateTopicAsync("kafka", "topic_name");





// kafka - надо вынести в конфигурацию
var kafkaQueueHandler = services.GetService<KafkaQueueHandler>();
string lastMessageStr =   kafkaQueueHandler.GetLastMessage(
                                                           configuration.GetSection("Settings:Kafka:broker").Value, 
                                                           configuration.GetSection("Settings:Kafka:topic").Value
                                                           );


int lastValue = 0;

// Вот здесь может все полететь !!!!!!
// Мессадж два раза повторяется.
// Таблица не вытаскивается из файла.
//  По идее ждать можно здесь все это !!
// Может рухнать - поменя название поля
// Отправка сообщения не правильно,должно крутиться постоянно,а не создавать обьект новый.
// название  string brokerList, string topicName не верно
// к сообщению при отправки в очередь не добавляется дата
// Б.Д. повторяется коннект, насколько это хорошо? -  Пораметры вынести как в первой отправке надо сделать. - Название нормально для отправки данных сделать.
// Для второй части надо ДЙ сделать.
// при получении данных,насколько уместно запихнули отправку сразуже в базу полученных данных ?
// Все что ниже вынести в одтедльный класс.
// Возможно рандом на лонг поменять.

try
{
    Message lastMessage = JsonSerializer.Deserialize<Message>(lastMessageStr);
    if (lastMessage != null)
    {
        lastValue = lastMessage != null ? lastMessage.Number + 1 : 0;
    }
} catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
 

NumberGenerator numberGenerator = new NumberGenerator();
KafkaProducer producer = new KafkaProducer();

while (true)
{
    for (int i = 0; i < 20; i++)
    {
        try
        {
            Message message = new Message()
            {
                Number = numberGenerator.GenerateElement(lastValue)
            };

            lastValue = message.Number;            
            await producer.SendMessage(message.ToString(), 
                                       configuration.GetSection("Settings:Kafka:broker").Value,
                                       configuration.GetSection("Settings:Kafka:topic").Value
                                       );

            await Task.Delay(lastValue % 17);              
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }        
    }

    await Task.Delay(1000 - DateTime.Now.Microsecond); 
}

 

