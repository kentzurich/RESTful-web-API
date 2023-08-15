namespace MagicVilla_VillaAPI.Logger
{
    public class Logging : ILogging
    {
        public void Log(string message, string? type = null)
        {
            if (type == "error")
                Console.Write($"ERROR - {message}");
            else
                Console.WriteLine(message);
        }
    }
}
