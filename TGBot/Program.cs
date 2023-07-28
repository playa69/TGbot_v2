using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

internal class Program
{
    private static void Main(string[] args)
    {
        ITelegramBotClient botClient;
        string TOKEN = "5970931742:AAEjllJ9lmYLYa7_6fr8FNCKRkOn3OGtYwc";
       
        
        botClient = new TelegramBotClient(TOKEN);

        var me = botClient.GetMeAsync().Result;
        Console.WriteLine(
            $"Мой ID: {me.Id}" +
            $", а имя {botClient.GetMeAsync().Result.FirstName}!"
            );

        
        


        string wk = DateTime.Today.DayOfWeek.ToString();
        TimeSpan msk = DateTime.Now.TimeOfDay;
        TimeSpan timeInParis = msk.Add(new TimeSpan(-2, 0, 0));

        botClient.OnMessage += BotClient_OnMessage;
        botClient.StartReceiving();
        Console.ReadKey();





        void BotClient_OnMessage(object sender, MessageEventArgs e)
        {

            var command = e.Message.Text;
           // var chatId = e.Message.Chat.Id;
           //var userName = e.Message.Chat.Username;

           //  botClient.SendTextMessageAsync(e.Message.Chat, " ssss");   // baluys'



            if (command != null)
            {
                
                if (command == "Какой сегодня день")
                {
                      botClient.SendTextMessageAsync(
                        e.Message.Chat,
                     $"{wk}" 
                        );
                }
                else if (command == "Сколько времени в Париже")
                {
                     botClient.SendTextMessageAsync(
                        e.Message.Chat,
                         $"В Париже {timeInParis.ToString(@"hh\:mm\:ss")}"
                        );
                }
                else if (command.StartsWith("Upper"))
                {
                      string up = command.Substring(6);
                      botClient.SendTextMessageAsync(
                        e.Message.Chat,
                    $"{up.ToUpper()}" 
                        );
                }
                else if (command.StartsWith("Reverse"))
                {
                    string up = command.Substring(8);
                    if (up != null)
                    {
                        char[] array = up.ToCharArray();
                        Array.Reverse(array);
                        up = new string(array);
                         botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat.Id,
                        text: $"{up}"                          
                            );
                    }
                }
                else
                {
                    
                    API animal;
                    switch (command)
                    {
                        case "киса":
                            animal = new Cat();
                            break;
                        case "доге":
                            animal = new Dog();
                            break;
                        default:
                            botClient.SendTextMessageAsync(
                                e.Message.Chat, "Не знаю("
                                );
                            return;
                    }

                    botClient.SendPhotoAsync(
                        e.Message.Chat,
                        animal.image,
                        animal.type
                        );

                }
            }
           
        }
        /*
        void BotClient_OnMessage1(object sender, MessageEventArgs e)
        {
            botClient.SendTextMessageAsync(e.Message.Chat,
                $"Привет!\n Ты написал: {e.Message.Text}");
             for (int i = 1; i < 400; i++)
                 botClient.SendTextMessageAsync(e.Message.Chat,
                 $"Blessings!");   
        }
        */
        
        /*
        void BotClient_OnMessage(object sender, MessageEventArgs e)
        {
            var webClient = new System.Net.WebClient();
            var json = webClient.DownloadString("https://dog.ceo/api/breeds/image/random ");
            webClient.Dispose();

            DogJsonSchema dog = JsonConvert.DeserializeObject<DogJsonSchema>(json);

            botClient.SendPhotoAsync(e.Message.Chat, dog.message);
            botClient.SendTextMessageAsync(e.Message.Chat, dog.status);
        }
        */
    }

}


public interface API
{
   public string image { get; }
   public string type { get; }
}


public class Dog : API
{
    static string API_URL = "https://dog.ceo/api/breeds/image/random";
    private class JsonSchema
    {
        public string message { get; set; }
        public string status { get; set; }    
    }

    public Dog()
    {
        var webClient = new System.Net.WebClient();
        var json = webClient.DownloadString(API_URL);
        webClient.Dispose();

        var data = JsonConvert.DeserializeObject<JsonSchema>(json);
        image = data.message;
    }
        
    public string image { get; }
    public string type
    {
        get
        {
          return "Dogggggggggggggyyyyyy";
        }
    }
}
public class Cat : API
{
    static string API_URL = "https://api.thecatapi.com/v1/images/search";
    private class JsonSchema
    {
        public string id { get; set; }
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        [JsonIgnore]
        public string[] breeds { get; set; }
    }

    public Cat()
    {
        var webClient = new System.Net.WebClient();
        var json = webClient.DownloadString(API_URL);
        webClient.Dispose();

        var data = JsonConvert.DeserializeObject<JsonSchema[]>(json)[0];
        image = data.url;
    }

    public string image { get; }
    public string type
    {
        get
        {
            return "Kiiiiiiiiiiiissssssaaa";
        }
    }
}


