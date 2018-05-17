using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Diagnostics;

namespace TranslationReplacer
{
    /// <summary>
    /// Class finds all references "@:" in .json file and replace it
    /// </summary>
    class Translator
    {
        // FIELDS AND PROPERTIES
        public string SourceFilePath { get; private set; }
        public string OutputFilePath { get; private set; }
        public JToken RootJsonObject { get; private set; }
        int totalErrors = 0;

        // CONSTRUCTORS
        public Translator(string sourceFilePath, string outputFilePath)
        {
            SourceFilePath = sourceFilePath;
            OutputFilePath = outputFilePath;
        }

        // METHODS

        /// <summary>
        /// Reads data from file .json
        /// </summary>
        private void ReadDataFromFile()
        {
            if (File.Exists(SourceFilePath) == false)
            {
                throw new FileNotFoundException("File does not exists or uncorrect file path");
            }
            try
            {
                using (StreamReader streamReader = File.OpenText(SourceFilePath))
                {
                    RootJsonObject = JToken.ReadFrom(new JsonTextReader(streamReader));
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("IO exception error " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception found " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void SaveDataToFile()
        {
            if (File.Exists(OutputFilePath))
            {
                Console.WriteLine("File overriden.");
            }
            else
            {
                Console.WriteLine("New file created.");
            }
            using (StreamWriter streamWriter = File.CreateText(OutputFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(streamWriter, RootJsonObject);
            }
        }
        /// <summary>
        /// Read, translate references and save file
        /// </summary>
        public void TranslateData()
        {
            // Read data from file
            ReadDataFromFile();

            // Check all data
            FindReferencesAndReplace(RootJsonObject);
            Console.WriteLine();
            Console.WriteLine($"Total errors in parsed document: {totalErrors}");
            Console.WriteLine();

            // Save data to faile
            SaveDataToFile();
        }

        /// <summary>
        /// Finds references and repleace all which need to be changed
        /// </summary>
        /// <param name="treeNode"></param>
        private void FindReferencesAndReplace(JToken treeNode)
        {
            if (treeNode.Type == JTokenType.Object)
            {
                foreach (JProperty child in treeNode.Children<JProperty>())
                {
                    FindReferencesAndReplace(child.Value);
                }
            }
            else if (treeNode.Type == JTokenType.String)
            {
                // check if value is a reference and if it is a reference find it and replace
                JValue currentJValue = (JValue)treeNode;
                string currentStringValue = (string)currentJValue.Value;

                if (currentStringValue.StartsWith("@:"))                                                    // check if it is an reference starting with "@:"
                {
                    string translationPath = currentStringValue.Substring(2);                               // remove "@:" from beggining of string to let JSON tree to parse this string directly

                    if (translationPath[translationPath.Length-1]=='.')                                     // check if there is unnecessary "." at the end of node chain
                    {
                        totalErrors++;
                        Console.WriteLine("UNNECESSARY DOT SIGN AT THE END OF PATH: " + translationPath);
                        translationPath = translationPath.TrimEnd('.');
                    }
                    
                    // finally find correct reference translation
                    string translatedValue = FindTranslationAndReplace(translationPath);

                    // replace reference translation
                    JValue nodeValue = (JValue)treeNode;
                    ReplaceSimpleValue(nodeValue, translatedValue);
                }
            }
            else
            {
                Console.WriteLine("Error, wrong value in JSON file.");
                return;
            }
        }

        /// <summary>
        /// Replaces value in leaf
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="translation"></param>
        private void ReplaceSimpleValue(JValue sourceValue, string translation)
        {
#if DEBUG
            //Console.WriteLine();
            //JToken parentBeforeTranslation = sourceValue.Parent;
            //Console.WriteLine($"Before translation: { parentBeforeTranslation}");
#endif

            sourceValue.Value = translation;

#if DEBUG
            //Console.WriteLine($"After translation: {(JToken)sourceValue.Parent}");
            //Console.WriteLine();
#endif
        }

        /// <summary>
        /// Finds translation in a tree and return it as a string
        /// </summary>
        /// <param name="translation"></param>
        private string FindTranslationAndReplace(string translationPath)
        {
            JToken selectedJToken = RootJsonObject.SelectToken(translationPath);                                        // returns token or null if doesn't exist

            // error case because of lack of element in tree structure
            if (selectedJToken == null)
            {
                totalErrors++;
                Console.WriteLine("TRANSLATION NOT FOUND: " + translationPath);
                return ("TRANSLATION NOT FOUND: " + translationPath);
            }
            // correct translation found:
            else if (selectedJToken.Type == JTokenType.String)
            {
                string translationFound = (string)selectedJToken;

                if (translationFound.StartsWith("@:"))
                {
                    string recursiveTranslationPath = translationFound.Substring(2);
                    return FindTranslationAndReplace(recursiveTranslationPath);
                }
                else
                {
                    return translationFound;
                }
            }
            // error case because of wrong element type
            else if (selectedJToken.Type == JTokenType.Object)
            {
                if (selectedJToken.HasValues)
                {
                    // prepare new path for repeated key feature
                    string[] tmpFullElementPath = translationPath.Split('.');
                    string lastElement = tmpFullElementPath[tmpFullElementPath.Length - 1];
                    string recursiveTranslationPath = $"{translationPath}.{lastElement}";

                    return FindTranslationAndReplace(recursiveTranslationPath);                                         // return shortcut value or not found if not exists
                }
                else
                {
                    totalErrors++;
                    Console.WriteLine("ERROR(path indicates at object), TRANSLATION NOT FOUND : " + translationPath);
                    return ("TRANSLATION NOT FOUND: " + translationPath);
                }
            }
            // other error case
            else
            {
                totalErrors++;
                Console.WriteLine("ERROR, TRANSLATION NOT FOUND: " + translationPath);
                return ("TRANSLATION NOT FOUND: " + translationPath);
            }
        }
    }
}
