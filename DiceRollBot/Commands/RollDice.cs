using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiceRollBot.Commands
{
    internal class RollDice : ICommand
    {
        public List<string> RunCommand(string message, string channel, string nick, GroupCollection parameters = null)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());

            int rolls = int.Parse(parameters[1].Value);
            int dieSize = int.Parse(parameters[2].Value);

            if(dieSize <= 0) return new List<string> {"Die size needs to be greater than 0"};

            if(rolls <= 0) return new List<string> {"There needs to be more than 0 rolls"};

            List<int> rollResults = new List<int>(rolls);

            int i = 0;
            while (i < rolls)
            {
                rollResults.Add(random.Next(1, dieSize + 1)); // It appears random won't pick the maximum value in Next so + 1 makes it possible for the highest value to be picked
                i++;
            }

            string result = $"{nick}'s roll results:";

            result = rollResults.Aggregate(result, (current, roll) => current + $" {roll},");

            result += $" Rolled {rolls}d{dieSize}";

            return new List<string> {result};
        }
    }
}
