using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    private DiscordSocketClient _client;
    private CommandService _commands;
    private IServiceProvider _services;

    // Define an array of predefined messages for random chance
    private string[] predefinedMessages = {
        "Hey, does anyone want to play Fortnite?",
        "Who's up for some Fortnite action?",
        "Fortnite time! Anyone interested?",
        "Ready to drop into some Fortnite matches?",
        "Looking for squadmates to play Fortnite!",
        "Fortnite enthusiasts, assemble!"
    };

    // Define an array of additional messages for when the bot is mentioned
    private string[] mentionedMessages = {
        "Sure thing! Let's play some Fortnite, {user}!",
        "Absolutely! I'm ready for Fortnite. How about you, {user}?",
        "Count me in! Fortnite sounds like a plan, {user}!",
        "Fortnite time! I'm game, {user}!",
        "Ready to conquer Fortnite together, {user}?",
        "{user}! Let's drop into some Fortnite matches!",
        "{user} I challenge you to a d-d-d-d-d-d-d-d-d-duel! in FORTNITE!",
        "{user} get your pickaxe sharpened and your dance moves ready? It's Fortnite time!"
    };

    static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

    public async Task RunBotAsync()
    {
        _client = new DiscordSocketClient();
        _commands = new CommandService();

        _client.Log += Log;

        await RegisterCommandsAsync();

        await _client.LoginAsync(TokenType.Bot, "BOT TOKEN HIDDEN");
        await _client.StartAsync();

        await _client.SetGameAsync("Fortnite"); // Set the bot's activity to "Playing Fortnite"

        await Task.Delay(-1);
    }

    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }

    public async Task RegisterCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;

        await _commands.AddModulesAsync(System.Reflection.Assembly.GetEntryAssembly(), null);
    }

    private async Task HandleCommandAsync(SocketMessage arg)
    {
        var message = arg as SocketUserMessage;
        var context = new SocketCommandContext(_client, message);

        if (message.Author.IsBot) return;

        int chance = new Random().Next(0, 100);
        if (chance < 15)
        {
            // Randomly choose and send one of the predefined messages
            int randomIndex = new Random().Next(0, predefinedMessages.Length);
            await context.Channel.SendMessageAsync(predefinedMessages[randomIndex]);
        }

        // Check if the bot is mentioned
        if (message.MentionedUsers.Any(x => x.Id == _client.CurrentUser.Id))
        {
            // Randomly choose and send one of the mentioned messages, mentioning the original user back
            int randomIndex = new Random().Next(0, mentionedMessages.Length);
            string mentionedMessage = mentionedMessages[randomIndex].Replace("{user}", context.User.Mention);
            await context.Channel.SendMessageAsync(mentionedMessage);
        }

        int argPos = 0;
        if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
        {
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
        }
    }
}
