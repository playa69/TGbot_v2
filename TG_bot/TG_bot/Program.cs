using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient(API_KEY);

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Начало обработки сообщений для  @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;
    var userName = message.Chat.Username;
    string wk = DateTime.Today.DayOfWeek.ToString();

    if (wk == "Wednesday")
    {
        wk = "Wednesday, ma dudes";
    }

    TimeSpan msk = DateTime.Now.TimeOfDay;
    TimeSpan timeInParis = msk.Add(new TimeSpan(-2, 0, 0));

    Console.WriteLine($"Получено сообщение '{messageText}' в чате номер {chatId} от пользователя {userName}.");

    var command = message.Text;

    if (command != null)
    {
        if (command == "Какой сегодня день")
        {
            Message sentMessage1 = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"{wk}",
                cancellationToken: cancellationToken
                );
        }
        else if (command == "Сколько времени в Париже")
        {
            Message sentMessage1 = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"В Париже {timeInParis.ToString(@"hh\:mm\:ss")}",
                cancellationToken: cancellationToken
                );
        }
        else if (command.StartsWith("Upper"))
        {
            string up = command.Substring(6);
            Message sentMessage1 = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"{up.ToUpper()}",
                cancellationToken: cancellationToken
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
                Message sentMessage1 = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"{up}",
                    cancellationToken: cancellationToken
                    );
            }
        }
        else
        {
            API animal;
            switch (command)
            {
                case "cat":
                    animal = new Cat();
                    break;
                case "dog":
                    animal = new Dog();
                    break;
                default:
                    Message sentMessage1 = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Непонятный запрос",
                        cancellationToken: cancellationToken
                        );
                    return;
            }
            Message sentMessage = await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: animal.image,
                caption: animal.type,
                cancellationToken: cancellationToken
            );
        }
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
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
            return "Песик";
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
        public int width { get; set; }
        public int height { get; set; }

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
            return "Котик";
        }
    }

}



