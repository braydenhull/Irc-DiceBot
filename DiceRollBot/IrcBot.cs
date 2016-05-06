using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Meebey.SmartIrc4net;
using Newtonsoft.Json;
using DiceRollBot.Commands;

namespace DiceRollBot
{
    internal class IrcBot
    {
        private IrcConfig _config;
        private static IrcClient IrcClient = new IrcClient();
        private string _configFile = "config.json";

        private List<Command> _commands = new List<Command>
        {
            new Command
            {
                CommandProvider = new RollDice(),
                Patterns = new  List<Regex>
                {
                    new Regex(@"^roll (\d+)d(\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                    new Regex(@"^roll (\d+)d(\d+) (\W)(\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                }
            }
        }; 

        internal IrcBot()
        {
            _config = LoadConfig();
            bool ssl = _config.ConnectionUri.Scheme == "ircs";

            IrcClient.AutoNickHandling = true;
            IrcClient.AutoRejoinOnKick = false;
            IrcClient.AutoReconnect = true;
            IrcClient.AutoRelogin = true;
            IrcClient.AutoRejoin = true;
            IrcClient.AutoJoinOnInvite = true;
            IrcClient.UseSsl = ssl;
            IrcClient.Encoding = Encoding.UTF8;
            IrcClient.CtcpVersion = "DiceBot/1.0";

            IrcClient.OnConnected += Connected;
            IrcClient.OnChannelMessage += OnChannelMessage;

            IrcClient.Connect(_config.ConnectionUri.IdnHost, _config.ConnectionUri.Port);
            IrcClient.Login(_config.Nick, _config.RealName, 0, _config.Username);
            IrcClient.Listen();
        }

        internal void Connected(object sender, EventArgs eventArgs)
        {
            if (_config.NickservPassword != null)
            {
                IrcClient.SendMessage(SendType.Message, "NickServ", $"IDENTIFY {_config.NickservPassword}");
            }

            foreach (string channel in _config.AutoJoinChannels)
            {
                IrcClient.RfcJoin(channel);
            }
        }

        internal void OnChannelMessage(object sender, IrcEventArgs eventArgs)
        {
            if (eventArgs.Data.Message[0] != _config.CommandPrefix) return;

            string nick = eventArgs.Data.Nick.Split('!')[0];
            string cleanedMessage = eventArgs.Data.Message.ToLower().TrimStart(_config.CommandPrefix);

            foreach (Command command in _commands)
            {
                foreach (Regex regex in command.Patterns)
                {
                    if (!regex.IsMatch(cleanedMessage)) continue;

                    try
                    {
                        foreach(string line in command.CommandProvider.RunCommand(cleanedMessage, eventArgs.Data.Channel, nick, regex.Match(cleanedMessage).Groups))
                        {
                            IrcClient.SendMessage(SendType.Message, eventArgs.Data.Channel, line);
                        }
                    }
                    catch (Exception e)
                    {
                        IrcClient.SendMessage(SendType.Message, eventArgs.Data.Channel, $"Such a simple bot and yet it still throws errors. Caught: {e.Source}: {e.Message}");
                    }
                }
            }
        }

        internal IrcConfig LoadConfig()
        {
            return JsonConvert.DeserializeObject<IrcConfig>(File.ReadAllText(_configFile));
        }

        internal void SaveConfig()
        {
            File.WriteAllText(_configFile, JsonConvert.SerializeObject(_config));
        }

        internal class Command
        {
            internal List<Regex> Patterns { get; set; } 
            internal ICommand CommandProvider { get; set; }
        }
    }

    public class IrcConfig
    {
        public Uri ConnectionUri { get; set; }
        public string RealName { get; set; } = "DiceBot";
        public string NickservPassword { get; set; } = null;
        public string Nick { get; set; } = "DiceBot";
        public string Username { get; set; } = "DiceBot";
        public char CommandPrefix { get; set; } = '!';

        public List<string> AutoJoinChannels { get; set; } 
    }
}
