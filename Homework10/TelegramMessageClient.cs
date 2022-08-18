using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
namespace Homework10
{
    class TelegramMessageClient
    {
        private MainWindow w;

        private static TelegramBotClient botClient;
        private string token = "5415590144:AAF00dfQJhKefmgRzL3sAaeepB_JYAcjCsk";
        private static Message message, sentMessage;
        private string messageText;
        private static long chatId;
        private static CancellationToken cancellationToken = new CancellationToken();

        public ObservableCollection<MessageLog> BotMessageLog { get; set; }

        private void MessageListener(long ID, string Name, string Msg)
        {
            

            

            w.Dispatcher.Invoke(() =>
            {
                BotMessageLog.Add(new MessageLog(
                    DateTime.Now.ToString()/*.ToLongDateString()*/,
                    Msg,
                    Name,
                    ID)
                    );
            });
        }

        public TelegramMessageClient(MainWindow W)
        {
            this.BotMessageLog = new ObservableCollection<MessageLog>();
            this.w = W;
            botClient = new TelegramBotClient(token);
            using var cts = new CancellationTokenSource();

            cancellationToken = cts.Token;

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            Console.WriteLine($"Начало прослушки @{botClient.GetMeAsync().Result.Username}");

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            Console.ReadLine();
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            message = update.Message;
            messageText = message.Text;
            chatId = message.Chat.Id;

            var messageType = message.Type;

            MessageListener(chatId, message.Chat.FirstName, messageText);

            Console.WriteLine($"Получено сообщение: '{messageText}' из чата {chatId}, {message.Chat.Username}. Содержит {messageType}");

            if (messageType == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                await GetTextMessage(messageText, cancellationToken);
            }
            else if (messageType == Telegram.Bot.Types.Enums.MessageType.Photo)
            {
                var fileID = update.Message.Photo[0].FileId;
                var fileInfo = await botClient.GetFileAsync(fileID);
                var filePath = fileInfo.FilePath;

                string[] fileExtension = filePath.Split('/')[1].Split('.');

                CheckDirectory(chatId.ToString());

                string destinationFilePath = $"{chatId}/{fileExtension[0]}.{fileExtension[1]}";
                await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                await botClient.DownloadFileAsync(
                    filePath: filePath,
                    destination: fileStream);


                await SendMessage("Выполнено", chatId);
            }
            else if (messageType == Telegram.Bot.Types.Enums.MessageType.Video)
            {
                var fileID = update.Message.Video.FileId;
                var fileInfo = await botClient.GetFileAsync(fileID);
                var filePath = fileInfo.FilePath;

                string[] fileExtension = filePath.Split('/')[1].Split('.');

                CheckDirectory(chatId.ToString());

                string destinationFilePath = $"{chatId}/{fileExtension[0]}.{fileExtension[1]}";
                await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                await botClient.DownloadFileAsync(
                    filePath: filePath,
                    destination: fileStream);


                await SendMessage("Выполнено", chatId);
            }
            else if (messageType == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                var fileID = update.Message.Document.FileId;
                var fileInfo = await botClient.GetFileAsync(fileID);
                var filePath = fileInfo.FilePath;

                string[] fileExtension = filePath.Split('/')[1].Split('.');

                CheckDirectory(chatId.ToString());

                string destinationFilePath = $"{chatId}/{fileExtension[0]}.{fileExtension[1]}";
                await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                await botClient.DownloadFileAsync(
                    filePath: filePath,
                    destination: fileStream);


                await SendMessage("Выполнено", chatId);
            }
            else if (messageType == Telegram.Bot.Types.Enums.MessageType.Audio)
            {
                var fileID = update.Message.Audio.FileId;
                var fileInfo = await botClient.GetFileAsync(fileID);
                var filePath = fileInfo.FilePath;

                string[] fileExtension = filePath.Split('/')[1].Split('.');

                CheckDirectory(chatId.ToString());

                string destinationFilePath = $"{chatId}/{fileExtension[0]}.{fileExtension[1]}";
                await using FileStream fileStream = System.IO.File.OpenWrite(destinationFilePath);
                await botClient.DownloadFileAsync(
                    filePath: filePath,
                    destination: fileStream);


                await SendMessage("Выполнено", chatId);
            }
            else
            {
                await SendMessage("Простите, я еще не умею работать с файлами данного типа(((", chatId);
            }
        }

        static async Task<Task> HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
        static async Task SendMessage(string text, long ID)
        {
            sentMessage = await botClient.SendTextMessageAsync(
                           chatId: ID,
                           text: text,
                           cancellationToken: cancellationToken);

            return;
        }


        static async Task GetTextMessage(string messageText, CancellationToken cancellationToken)
        {

            Message sentMessage;

            if (messageText.ToLower().Contains("скач") || messageText.ToLower().Contains("download"))
            {
                CheckDirectory(chatId.ToString());
                string[] splitedArr = messageText.Split(' ');
                try
                {
                    await using Stream stream = System.IO.File.OpenRead($@"../net5.0/{chatId}/{splitedArr[1]}");
                    sentMessage = await botClient.SendDocumentAsync(
                        chatId: chatId,
                        document: new InputOnlineFile(content: stream, fileName: splitedArr[1]),
                        cancellationToken: cancellationToken);
                }
                catch
                {
                    await SendMessage("Файл не найден или пуст!", chatId);
                }

                return;
            }
            Random rnd = new Random();
            string[] textArr = messageText.Split(' ');
            string otherText = "";
            for (int i = 1; i < textArr.Length; i++)
                otherText += $" {textArr[i]}";

            switch (textArr[0].ToLower())
            {
                case "/start":
                    await SendMessage("Привет! Вот что я умею:\n" +
                                 "help или помощь - напоминю что я умею\n" +
                                 "8ball или шар [текст] - потрясу волшебный шар\n" +
                                 "chance или шанс [текст] - шанс на что угодно\n" +
                                 "files или файлы - список файлов, доступных для скачивания\n" +
                                 "download или скач [имя файла] - скачать файл, если такой, конечно, существует\n" +
                                 "Также можете отправить файлы и я их сохраню чтобы Вы могли их скачать позже. ", chatId);
                    break;

                case "help":
                case "помощь":
                    await SendMessage("Вот что я умею:\n" +
                                 "help или помощь - напоминю что я умею\n" +
                                 "8ball или шар [текст] - потрясу волшебный шар\n" +
                                 "chance или шанс [текст] - шанс на что угодно\n" +
                                 "files или файлы - список файлов, доступных для скачивания\n" +
                                 "download или скач [имя файла] - скачать файл, если такой, конечно, существует\n" +
                                 "Также можете отправить файлы и я их сохраню чтобы Вы могли их скачать позже", chatId);

                    break;

                case "8ball":
                case "шар":
                    string[] answers = {"Бесспорно",
                                        "Предрешено",
                                        "Никаких сомнений",
                                        "Определённо да",
                                        "Можешь быть уверен в этом",
                                        "Мне кажется — «да»",
                                        "Вероятнее всего",
                                        "Хорошие перспективы",
                                        "Знаки говорят — «да»",
                                        "Да",
                                        "Пока не ясно, попробуй снова",
                                        "Спроси позже",
                                        "Лучше не рассказывать",
                                        "Сейчас нельзя предсказать",
                                        "Сконцентрируйся и спроси опять",
                                        "Даже не думай",
                                        "Мой ответ — «нет»",
                                        "По моим данным — «нет»",
                                        "Перспективы не очень хорошие",
                                        "Весьма сомнительно"};
                    await SendMessage(answers[rnd.Next(0, 19)], chatId);
                    break;

                case "chance":
                case "шанс":
                    await SendMessage($"Ваш шанс на{otherText}: {rnd.Next(0, 100)}%", chatId);
                    break;

                case "files":
                case "файлы":
                    string files = "";
                    CheckDirectory(chatId.ToString());
                    string[] filesArr = Directory.GetFiles(chatId + "/");

                    foreach (string s in filesArr)
                    {

                        files += s.Remove(0, chatId.ToString().Length + 1) + "\n";
                    }

                    await SendMessage($"Список файлов доступных для скачивания:\n" + files, chatId);
                    break;

                default:
                    await SendMessage($"Простите, но когда-то я научусь вас понимать(((", chatId);
                    break;
            }
            otherText = "";
            return;
        }

        static void CheckDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void SendSMS(string content, long ID)
        {
            SendMessage(content, ID);
        }
    }
}
