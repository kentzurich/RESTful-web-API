namespace MagicVilla_VillaAPI.Logger
{
    public class LoggingV2 : ILogging
    {
        public void Log(string message, string? type = null)
        {
            if(type == "error")
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR - {message}");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            else
            {
                if(type == "warning")
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"WARNING - {message}");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }
    }
}
