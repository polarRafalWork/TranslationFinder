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
        public JObject rootJsonObject { get; private set; }
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
                    rootJsonObject = (JObject)JToken.ReadFrom(new JsonTextReader(streamReader));
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

            // Console.WriteLine(jsonObject);
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

        public void TranslateData()
        {
            // Read data from file
            ReadDataFromFile();

            // Check all data
            FindReferencesAndReplace(rootJsonObject);

            // Save data to faile
            SaveDataToFile(OutputFilePath);
        }

        private void FindReferencesAndReplace(JObject treeNode)
        {

            // Find references and repleace all which need to be changed
            IEnumerator<KeyValuePair<string, JToken>> enumerator = treeNode.GetEnumerator();
            Console.WriteLine("mam: " + treeNode.Count + " dzieci");
            Type t = typeof(JValue);

            Console.WriteLine(t);

            if (treeNode.HasValues)
            {
                // foreach node findAndReplace
                while (enumerator.MoveNext())
                {
                        JObject currentObject = (JObject)enumerator.Current.Value;
                        Console.WriteLine("Jestem w : " + enumerator.Current.Key);
                        FindReferencesAndReplace(currentObject);
                }
            }
            else
            {
                // check if value is a reference and if its reference find and replace
                string currentValue = (string)enumerator.Current.Value;
                if (currentValue.Contains("@:"))
                {
                    if (currentValue.StartsWith("@:"))
                    {
                        Console.WriteLine(currentValue);
                        Translation(currentValue);
                    }
                }
                
            }

            




            //// Translate objects
            ////Console.WriteLine(jsonObject.Root);
            ////foreach (JToken token in jsonObject)
            ////{
            ////Dictionary<string, JToken>
            ////}

            //jsonContainer = rootJsonObject;
            //JEnumerable<JToken> containerChildren = jsonContainer.Children();



            //Console.WriteLine(rootJsonObject.Count);
            //JEnumerable<JToken> childrenList = rootJsonObject.Children();

            //IEnumerator<KeyValuePair<string, JToken>> enumerator = rootJsonObject.GetEnumerator();
            //Console.WriteLine(enumerator.Current);

            //enumerator.MoveNext();
            //Console.WriteLine(enumerator.Current.Key);






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
