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
            bool modifier = false;
            bool subtract = false;
            bool add = false;
            int modifierValue = 0;

            if (parameters.Count > 3)
            {
                modifier = true;
                if (parameters[3].Value == "+") add = true;
                if (parameters[3].Value == "-") subtract = true;
                if (!add && !subtract)
                {
                    return new List<string> {"Please provide a valid operator of either + or -"};
                }
                modifierValue = int.Parse(parameters[4].Value);
            }

            if(dieSize <= 0) return new List<string> {"Die size needs to be greater than 0"};

            if(rolls <= 0) return new List<string> {"There needs to be more than 0 rolls"};

            List<int> rollResults = new List<int>(rolls);

            int i = 0;
            int currentResult = 0;
            while (i < rolls)
            {
                currentResult = random.Next(1, dieSize + 1); // It appears random won't pick the maximum value in Next so + 1 makes it possible for the highest value to be picked
                if (modifier)
                {
                    if (add) currentResult = currentResult + modifierValue;
                    if (subtract) currentResult = currentResult - modifierValue;
                }

                rollResults.Add(currentResult);

                i++;
            }

            string result = $"{nick}'s roll results:";

            result = rollResults.Aggregate(result, (current, roll) => current + $" {roll},");

            result += $" Rolled {rolls}d{dieSize}";
            if (modifier)
            {
                if (add)
                {
                    result += $" with a +{modifierValue} modifier";
                }
                else if (subtract)
                {
                    result += $" with a -{modifierValue} modifier";
                }
            }

            return new List<string> {result};
        }
    }
}
