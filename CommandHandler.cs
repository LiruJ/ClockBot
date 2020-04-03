using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HourTrackerBot
{
    public class CommandHandler
    {
        #region Dependencies
        private readonly CommandService commandService;
        private readonly DiscordSocketClient discordClient;
        private readonly IServiceProvider serviceProvider;


        #endregion

        #region Constructors
        public CommandHandler(CommandService commandService, DiscordSocketClient discordClient, IServiceProvider serviceProvider)
        {
            // Set dependencies.
            this.commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            this.discordClient = discordClient ?? throw new ArgumentNullException(nameof(discordClient));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            // Initialise.
            initialiseCommands();
        }
        #endregion

        #region Initialisation Functions
        private void initialiseCommands()
        {
            discordClient.MessageReceived += handleCommandAsync;
        }
        #endregion

        #region Command Functions
        private async Task handleCommandAsync(SocketMessage message)
        {
            // Get the context of the message.
            SocketCommandContext context = new SocketCommandContext(discordClient, message as SocketUserMessage);

            // The arg pos.
            int argPos = 0;

            // If the message has the prefix, do the command.
            if ((message as SocketUserMessage).HasCharPrefix('.', ref argPos))
            {
                await commandService.ExecuteAsync(context, 1, serviceProvider);
            }
        }
        #endregion
    }
}
