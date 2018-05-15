using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationReplacer
{
    class Program
    {
        static void Main(string[] args)
        {

            string fileSourcePath = $"C:/Users/polar/source/repos/TranslationFinder/TranslationReplacer/TranslationReplacer/SourceTranslationFiles/locale-de-DE.json";
            
            string fileOutputPath = $"C:/Users/polar/source/repos/TranslationFinder/TranslationReplacer/TranslationReplacer/SourceTranslationFiles/locale-de-DE_output.json";
            Translator translator = new Translator(fileSourcePath, fileOutputPath);
            translator.TranslateData();
            

            Console.ReadLine();
        }
    }
}
