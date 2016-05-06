# Irc-DiceBot
C# bot for rolling dice on IRC

## Commands
There is only one command

Remember to use the command prefix, which is set in the config. It defaults to !. For example, the roll command could be !roll or @roll depending on the configuration.

### Roll
`roll <num>d<num>`

`roll <num>d<num> <modifier><num>`

Rolls a dice the given number of times for the given number.

e.g. to roll a D20 5 times, `roll 5d20`.

e.g. to rol la D20 5 times with +2 modifier, `roll 5d20 +2`

Acceptable modifiers are + and -.

Commands are not case sensitive.

## Credits
* meebey for SmartIrc4net
* James Newton-King for JSON.NET
* Xeron for the idea