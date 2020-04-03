using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace HourTrackerBot
{
    class Program
    {
        #region Constants
        private const string botToken = "";

        private const string xmlPath = "";
        #endregion

        #region Fields
        private DiscordSocketClient client;

        /// <summary> The first server the bot joins. </summary>
        private SocketGuild firstServer;

        private CommandHandler commandHandler;

        private UserManager userManager;

        private IServiceProvider services;
        #endregion

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            // Create the services.
            client = new DiscordSocketClient(new DiscordSocketConfig() { LogLevel = LogSeverity.Verbose, WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance });

            // Bind events.
            client.GuildAvailable += joinedServerAsync;
            client.JoinedGuild += joinedServerAsync;

            client.Log += Log;

            // Register commands and log in.
            await client.LoginAsync(Discord.TokenType.Bot, botToken);
            await client.StartAsync();

            // Block until the program is closed.
            await Task.Delay(-1);
        }

        private async Task joinedServerAsync(SocketGuild server)
        {
            if (firstServer is null)
            {
                firstServer = server;

                CommandService commandService = new CommandService();


                // Create user manager.
                userManager = new UserManager(firstServer, client);
                userManager.LoadFromFile(xmlPath);

                // Create the services collection and build it.
                services = new ServiceCollection().AddSingleton(client).AddSingleton(commandService).AddSingleton(userManager).BuildServiceProvider();

                // Set up the command handler.
                await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
                commandHandler = new CommandHandler(commandService, client, services);

                // Log the login.
                Console.WriteLine("ClockBot logged in successfully.");
            }
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }
    }
}
