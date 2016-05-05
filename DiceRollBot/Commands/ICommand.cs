using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiceRollBot.Commands
{
    internal interface ICommand
    {
        List<string> RunCommand(string message, string channel, string nick, GroupCollection parameters = null);
    }
}
