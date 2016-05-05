using System;

namespace DiceRollBot
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += Restart;
            
            IrcBot ircBot = new IrcBot();
        }

        static void Restart(object sender, UnhandledExceptionEventArgs args)
        {
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location,
                string.Join(" ", Environment.GetCommandLineArgs()));

            Environment.Exit(1);
        }
    }
}
