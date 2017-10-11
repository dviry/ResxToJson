using Croc.DevTools.ResxToJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;

namespace JsonToResx.Cli
{
    public class JsonToResxConverter
    {
        public static ConverterLogger Convert(JsonToResxConverterOptions options)
        {
            var logger = new ConverterLogger();

            Dictionary<string, ResXResourceWriter> resXfiles = new Dictionary<string, ResXResourceWriter>();

            List<string> key = new List<string>();
            JsonTextReader reader = new JsonTextReader(new StringReader(File.ReadAllText(options.Inputs.First())));
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.Comment)
                    continue;

                if (reader.Value != null)
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        key.Add(reader.Value.ToString());
                        var next = reader.Read();
                        if (reader.TokenType == JsonToken.String)
                        {
                            string value = reader.Value.ToString();
                            string keyname = $"{String.Join(options.KeySeparator, key.Skip(1))}";
                            Console.WriteLine($"{keyname} = {value}");
                            ResXResourceWriter resX = getResX(options, resXfiles, key[0]);
                            resX.AddResource(keyname, value);
                            key.Remove(key.Last());
                            continue;
                        }
                        if (reader.TokenType == JsonToken.StartArray)
                        {
                            int i = 0;
                            while (reader.Read())
                            {
                                if (reader.TokenType == JsonToken.EndArray)
                                    break;

                                string value = reader.Value.ToString();
                                string keyname = $"{String.Join(options.KeySeparator, key.Skip(1))}{options.KeySeparator}{i}";
                                Console.WriteLine($"{keyname} = {value}");
                                ResXResourceWriter resX = getResX(options, resXfiles, key[0]);
                                resX.AddResource(keyname, value);
                                i++;
                            }
                            key.Remove(key.Last());
                            continue;
                        }
                    }
                }
                else if (key.Count > 0)
                {
                    key.Remove(key.Last());
                }
            }

            foreach (var resX in resXfiles.Values)
            {
                resX.Close();
            }
            return logger;
        }

        private static ResXResourceWriter getResX(JsonToResxConverterOptions options, Dictionary<string, ResXResourceWriter> resXfiles, string lang)
        {
            ResXResourceWriter resX;
            if (!resXfiles.TryGetValue(lang, out resX))
            {
                resX = new ResXResourceWriter(getResXName(options, lang));
                resXfiles.Add(lang, resX);
            }

            return resX;
        }

        private static string getResXName(JsonToResxConverterOptions options, string lang)
        {
            string fileName = options.OutputFile;

            if (lang != options.FallbackCulture)
            {
                fileName = $"{Path.GetFileNameWithoutExtension(fileName)}.{lang}{Path.GetExtension(fileName)}";
            }

            return Path.Combine(options.OutputFolder, fileName);            
        }
    }
}
