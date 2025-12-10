using dotenv.net;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

DotEnv.Load();
var envVars = DotEnv.Read();
var cts = new CancellationTokenSource();
var client = new TelegramBotClient(envVars["TG_TOKEN"], cancellationToken: cts.Token);

var me = await client.GetMe();
Console.WriteLine(me);

client.OnMessage += OnMessage;
client.OnUpdate += OnUpdate;

async Task OnCallback(CallbackQuery query)
{
    int index = query.Data!.LastIndexOf('_');
    string data = query.Data;
    if (index != -1)
    {
        data = query.Data.Substring(0, index);
    }
    if (data != "empty")
    {
        await client.DeleteMessage(query.Message!.Chat, query.Message!.Id);
    }

    switch (data)
    {
        case "city":
            await client.SendMessage(query.Message!.Chat, "Выберете тип документа",
                replyMarkup: new InlineKeyboardButton[][]
                {
                    [("Ознакомление с материалами дела", "document_type_1")],
                    [(" ", "empty")],
                    [(" ", "empty")]
                });
            break;
        case "document_type":
            await client.SendMessage(query.Message!.Chat, "Выберете суд",
                replyMarkup: new InlineKeyboardButton[][]
                {
                    [("Суд общей юридикиции", "judge_1_1")],
                    [("Арбитражный суд", "empty")]
                });
            break;
        case "judge_1":
            await client.SendMessage(query.Message!.Chat, "Выберете суд",
                replyMarkup: new InlineKeyboardButton[][]
                {
                    [("Районный суд", "judge_2_1")],
                    [("Областной суд", "empty")],
                    [("Мировой суд", "empty")]
                });
            break;
        case "judge_2":
            await client.SendMessage(query.Message!.Chat, "Выберете суд",
                replyMarkup: new InlineKeyboardButton[][]
                {
                    [("Верх-исетский", "judge_3_1")],
                    [("Кировский", "empty")],
                    [("Октябрьский", "empty")],
                    [("Железно-дорожный", "empty")],
                    [("Ленинский", "empty")]
                });
            break;
        case "judge_3":
            await client.SendMessage(query.Message!.Chat, "Укажите номер дела");
            break;
        case "default":
            break;
    }
    Console.WriteLine($"{query.Data} | {query.Message?.Text}");
}

async Task OnUpdate(Update update)
{
    if (update is Update { CallbackQuery: { } callbackQuery })
    {
        await client.AnswerCallbackQuery(callbackQuery.Id);
        await OnCallback(callbackQuery);
    }

}

async Task OnMessage(Message message, UpdateType type)
{
    if (message.Text is null) return;

    var msg = message.Text;

    switch (msg)
    {
        case "/start":
            await client.SendMessage(message.Chat.Id, "Приветствую", replyMarkup: new KeyboardButton[][]
            {
                ["Создать юридический документ"],
                ["Справочная информация",
                "Помощь"]
            });
            break;
        case "Создать юридический документ":
            await client.SendMessage(message.Chat.Id, "Выберите город",
                replyMarkup: new InlineKeyboardButton[][]
                {
                    [("Екатеринбург", "city_1")],
                    [(" ", "empty")],
                    [(" ", "empty")],
                    [(" ", "empty")],
                });
            break;
        case "Справочная информация":
        case "Помощь":
            await client.SendMessage(message.Chat.Id, "not implemented");
            break;
        default:
            break;

    }

}

Console.ReadLine();
cts.Cancel();


