using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace TranslationReplacer
{
    class Translator
    {
        public string SourceFilePath { get; private set; }
        public string OutputFilePath { get; private set; }
        public JToken rootJsonObject { get; private set; }
        public JContainer jsonContainer { get; set; }

        public Translator(string sourceFilePath, string outputFilePath)
        {
            SourceFilePath = sourceFilePath;
            OutputFilePath = outputFilePath;
        }



        private void ReadDataFromFile()
        {
            if (File.Exists(SourceFilePath) == false)
            {
                throw new FileNotFoundException("File does not exists or uncorrect file path");
            }
            try
            {
                //using (StreamReader streamReader = new StreamReader(SourceFilePath))
                //{
                //    string jsonInputString = streamReader.ReadToEnd();
                //    jsonObject = (JObject)JsonConvert.DeserializeObject(jsonInputString);
                //}

                using (StreamReader streamReader = File.OpenText(SourceFilePath))
                {
                    rootJsonObject = JToken.ReadFrom(new JsonTextReader(streamReader));
                }

            }
            catch (IOException ex)
            {
                Console.WriteLine("IO error");
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception found");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void SaveDataToFile(string outputFilePath)
        {
            OutputFilePath = outputFilePath;

#if DEBUG
            using (StreamWriter streamWriter = File.CreateText(OutputFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(streamWriter, rootJsonObject);
            }
#elif !DEBUG
            int numberOfFileVersion = 1;
            while (File.Exists(OutputFilePath))
            {
                OutputFilePath += numberOfFileVersion.ToString();
                numberOfFileVersion++;
            }

            Console.WriteLine("##############################################################################################################################");
            Console.WriteLine("JSON AFTER TRANSLATION");
            Console.WriteLine("##############################################################################################################################");
            Console.WriteLine(jsonObject);


            using (StreamWriter streamWriter = File.CreateText(OutputFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(streamWriter, jsonObject);
            }
#endif
        }
        /// <summary>
        /// Read, translate references and save file
        /// </summary>
        public void TranslateData()
        {
            // Read data from file
            ReadDataFromFile();

            // Check all data
            FindReferencesAndReplace(rootJsonObject);

            // Save data to faile
            SaveDataToFile(OutputFilePath);
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
            else if (treeNode.Type == JTokenType.String)  // czy zawsze będą tylko stringi?
            {
                // check if value is a reference and if its reference find and replace
                JValue currentJValue = (JValue)treeNode;
                string currentStringValue = (string)currentJValue.Value;

                if (currentStringValue.StartsWith("@:"))
                {
                    Console.WriteLine(currentStringValue);
                    string translationPath = currentStringValue.Substring(2);
                    Translation(translationPath);
                }
            }
            else
            {
                Console.WriteLine("Error, wrong value in JSON file.");
                return;
            }
        }

        /// <summary>
        /// Finds translation in a tree and return it
        /// </summary>
        /// <param name="translation"></param>
        private string Translation(string translation)
        {
            string result = "";


            return result;
        }
    }
}
