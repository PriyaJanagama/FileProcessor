using System;
using System.IO;

namespace FileProcessor
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                return;
            }
            var fileName = args[0];
            if (!File.Exists(fileName))
            {
                Console.WriteLine(string.Format("Input file not found - {0}", fileName));
                Console.ReadKey();
                return;
            }
            var fileProcessor = new FileProcessor();
            fileProcessor.Run(fileName);
        }
    }
}
