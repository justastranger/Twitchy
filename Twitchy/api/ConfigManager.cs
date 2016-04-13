using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Twitchy
{
    public class ConfigManager
    {
        private static FileStream configStream = File.Open("config.json", FileMode.OpenOrCreate);
        public static JObject configs;
        public static ConfigManager instance = new ConfigManager();

        static ConfigManager()
        {   // temp value
            string unparsed;
            // read our config file
            using (StreamReader sr = new StreamReader(configStream))
            {
                unparsed = sr.ReadToEnd();
                sr.Close();
            }
            // If it's not a new file, load it (pretending it's valid)
            if (unparsed != "") configs = JObject.Parse(unparsed);
            else {
                // otherwise create a blank config
                configs = new JObject();
                // and write it to our config file
                configStream = File.Open("config.json", FileMode.Create);
                using (StreamWriter sw = new StreamWriter(configStream))
                {
                    sw.Write(configs.ToString());
                    sw.Close();
                }
            }
        }

        // Called by our plugins when they want to be included in the config file
        public JObject registerPlugin(string name, JObject config)
        {   // Check if they're registered or not
            // if they aren't, add the config object they provide to the global config object
            if (configs[name] == null) configs[name] = config;
            // return whatever is currently in the global config object
            configStream = File.Open("config.json", FileMode.Create);
            using (StreamWriter sw = new StreamWriter(configStream))
            {
                sw.Write(configs.ToString());
                sw.Close();
            }
            return (JObject)configs[name];
        }
    }
}
