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
                string fileSourcePath = args[0];
                string fileOutputPath = args[1];
                Translator translator = new Translator(fileSourcePath, fileOutputPath);
                translator.TranslateData();

                Console.WriteLine("Please enter any key...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Incorrect number of parametres (accepted parameters: sourceFilePath, outputFilePath)");
                Console.WriteLine("Please enter any key...");
                Console.ReadLine();
            }
#else
            string fileSourcePath = @"C:/Users/rpolar/Desktop/GIT/TranslationFinder/TranslationReplacer/TranslationReplacer/SourceTranslationFiles/locale-de-DE.json";
            string fileOutputPath = @"C:/Users/rpolar/Desktop/GIT/TranslationFinder/TranslationReplacer/TranslationReplacer/SourceTranslationFiles/locale-de-DE_output.json";
            Translator translator = new Translator(fileSourcePath, fileOutputPath);
            translator.TranslateData();

            Console.ReadLine();
#endif
        }
    }
}
