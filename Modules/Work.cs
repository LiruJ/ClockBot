using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HourTrackerBot.Modules
{
    public class Work : ModuleBase<SocketCommandContext>
    {
        private readonly UserManager userManager;

        public Work(UserManager userManager)
        {
            this.userManager = userManager;
        }

        [Command("work")]
        public async Task WorkAsync(string workType)
        {
            // Get the user.
            WorkerUser workerUser = userManager.GetUserFromID(Context.User.Id);

            // Handle the type.
            switch (workType.ToLower())
            {
                case "start":
                    await workerUser.StartWorkingAsync(Context.Channel);
                    break;
                case "stop":
                    await workerUser.StopWorkingAsync(Context.Channel);

                    // Save the user data.
                    userManager.Save();
                    break;
                case "total":
                    TimeSpan totalWorkTime = workerUser.CalculateTotalTimeWorked();
                    await ReplyAsync($"You have worked a total of {totalWorkTime.Days} days, {totalWorkTime.Hours} hours, and {totalWorkTime.Minutes} minutes.");
                    break;
                default:
                    await ReplyAsync("Parameter not recoginised, try \"start\" or \"stop\".");
                    return;
            }
        }
    }
}
