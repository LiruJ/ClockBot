using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HourTrackerBot
{
    public class WorkerUser
    {
        #region Dependencies
        private readonly SocketGuildUser guildUser;
        private readonly DiscordSocketClient botUser;
        private readonly XmlNode userNode;

        #endregion

        #region Fields
        private List<ValueTuple<DateTime, DateTime>> startAndEndTimes;

        private bool isWorking = false;

        private DateTime workStartTime;
        #endregion

        #region Properties
        public string Username { get => guildUser.Username; }
        #endregion

        #region Constructors
        public WorkerUser(SocketGuildUser guildUser, DiscordSocketClient botUser, XmlNode userNode)
        {
            this.guildUser = guildUser ?? throw new ArgumentNullException(nameof(guildUser));
            this.botUser = botUser ?? throw new ArgumentNullException(nameof(botUser));
            this.userNode = userNode ?? throw new ArgumentNullException(nameof(userNode));

            loadTimes();
        }
        #endregion

        #region Load Functions
        private void loadTimes()
        {
            startAndEndTimes = new List<(DateTime, DateTime)>(userNode.ChildNodes.Count);

            foreach (XmlNode timeNode in userNode.ChildNodes)
            {
                DateTime startTime = DateTime.Parse(timeNode.Attributes.GetNamedItem("Start").Value);
                DateTime endTime = DateTime.Parse(timeNode.Attributes.GetNamedItem("End").Value);

                startAndEndTimes.Add((startTime, endTime));
            }
        }
        #endregion

        #region Save Functions
        public void SaveTime(DateTime startTime, DateTime endTime)
        {
            // Create a new time node.
            XmlElement timeNode = userNode.OwnerDocument.CreateElement("Time");
            timeNode.SetAttribute("Start", startTime.ToString("dd/MM/yy hh:mm"));
            timeNode.SetAttribute("End", endTime.ToString("dd/MM/yy hh:mm"));

            // Append the new time node to the user node.
            userNode.AppendChild(timeNode);
        }
        #endregion

        #region Work Functions
        public async Task StartWorkingAsync(ISocketMessageChannel messageChannel)
        {
            // If the user is already working, don't allow them to start working twice.
            if (isWorking)
            {
                await messageChannel.SendMessageAsync("You are already working.");
                return;
            }

            isWorking = true;
            workStartTime = DateTime.Now;

            await messageChannel.SendMessageAsync($"{guildUser.Username} has started work at {workStartTime.ToShortTimeString()}");

        }

        public async Task StopWorkingAsync(ISocketMessageChannel messageChannel)
        {
            // If the user is not working, don't allow them to stop working.
            if (!isWorking)
            {
                await messageChannel.SendMessageAsync("You are not working.");
                return;
            }

            isWorking = false;
            DateTime workEndTime = DateTime.Now;

            startAndEndTimes.Add((workStartTime, workEndTime));

            SaveTime(workStartTime, workEndTime);

            await messageChannel.SendMessageAsync($"{guildUser.Username} has stopped work at {workEndTime.ToShortTimeString()}, {(workEndTime - workStartTime).Hours} hours and {(workEndTime - workStartTime).Minutes} minutes.");
        }

        public TimeSpan CalculateTotalTimeWorked()
        {
            TimeSpan totalTime = new TimeSpan();

            foreach ((DateTime start, DateTime end) in startAndEndTimes)
            {
                totalTime += end - start;
            }

            return totalTime;
        }
        #endregion
    }
}
