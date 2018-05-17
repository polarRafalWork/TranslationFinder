using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TranslationReplacer
{
    class Program
    {
        static void Main(string[] args)
        {
#if !DEBUG
            if(args.Length == 2)
            {
                //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                //stopwatch.Start();

                string fileSourcePath = args[0];
                string fileOutputPath = args[1];
                Translator translator = new Translator(fileSourcePath, fileOutputPath);
                Console.WriteLine();
                translator.TranslateData();

                //stopwatch.Stop();
                //long ticksNumber = stopwatch.ElapsedTicks;
                //long elapsedTime = stopwatch.ElapsedMilliseconds;

                //Console.WriteLine();
                //Console.WriteLine($"Total ticks number: {ticksNumber}");
                //Console.WriteLine($"Elapsed time (ms): {elapsedTime}");
                Console.WriteLine();

            }
            else
            {
                Console.WriteLine("Incorrect number of parametres (accepted parameters: sourceFilePath, outputFilePath)");
                Console.WriteLine("Please enter any key...");
                Console.ReadLine();
            }
#else

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();


            string fileSourcePath = @"C:/Users/rpolar/Desktop/GIT/TranslationFinder/TranslationReplacer/TranslationReplacer/SourceTranslationFiles/locale-de-DE.json";
            string fileOutputPath = @"C:/Users/rpolar/Desktop/GIT/TranslationFinder/TranslationReplacer/TranslationReplacer/SourceTranslationFiles/locale-de-DE_output.json";
            Translator translator = new Translator(fileSourcePath, fileOutputPath);
            translator.TranslateData();

            Console.WriteLine();
            Console.WriteLine();


            stopwatch.Stop();
            long ticksNumber = stopwatch.ElapsedTicks;
            long elapsedTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Total ticks number: {ticksNumber}");
            Console.WriteLine($"Elapsed time (ms): {elapsedTime}");

            Console.ReadLine();
#endif
        }
    }
}
