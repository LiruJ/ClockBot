using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HourTrackerBot
{
    public class UserManager
    {
        #region Dependencies
        private readonly SocketGuild server;
        private readonly DiscordSocketClient botUser;

        #endregion

        #region Fields
        private Dictionary<ulong, WorkerUser> usersByID;

        private string documentFilePath = "";

        private XmlDocument userDocument;
        #endregion

        #region Constructors
        public UserManager(SocketGuild server, DiscordSocketClient botUser)
        {
            // Initialise collections.
            usersByID = new Dictionary<ulong, WorkerUser>(4);
            this.server = server ?? throw new ArgumentNullException(nameof(server));
            this.botUser = botUser ?? throw new ArgumentNullException(nameof(botUser));
        }
        #endregion

        #region Load Functions
        public void LoadFromFile(string filePath)
        {
            documentFilePath = filePath;

            userDocument = new XmlDocument();
            userDocument.Load(filePath);

            // Get the root node.
            XmlNode rootNode = userDocument.LastChild;

            // Go over each node within this node and parse it.
            foreach (XmlNode userNode in rootNode.ChildNodes)
            {
                ulong userID = ulong.Parse(userNode.Attributes.GetNamedItem("ID").Value);

                usersByID.Add(userID, new WorkerUser(server.GetUser(userID), botUser, userNode));

                Console.WriteLine($"Loaded and added {usersByID[userID].Username} to data.");
            }
        }
        #endregion

        #region Save Functions
        public void Save()
        {
            userDocument.Save(documentFilePath);
            Console.WriteLine($"Saved user data to {documentFilePath}");
        }
        #endregion

        #region Get Functions
        public WorkerUser GetUserFromID(ulong ID)
        {
            return usersByID[ID];
        }
        #endregion
    }
}
