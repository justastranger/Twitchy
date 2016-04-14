using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Twitchy.api;

namespace Twitchy
{
    class PluginManager
    {

        public static Dictionary<string, Plugin> plugins = new Dictionary<string, Plugin>();
        
        public static void initializePages()
        {
            foreach (KeyValuePair<string, Plugin> plugin in plugins)
            {
                TabPage tmp = plugin.Value.getPage();
                if (tmp!=null) context.twitchy.addPage(tmp);
            }
        }

        public static void loadPlugins()
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"plugins\")) Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\plugins\");
            string[] pluginPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\plugins\");
            

            foreach (string plugin in pluginPaths)
            {
                // Only load dll's
                if (!plugin.EndsWith(".dll")) continue;
                Assembly assembly = (Assembly.LoadFile(plugin));
                Plugin pluginInstance = (Plugin)assembly.CreateInstance(Path.GetFileNameWithoutExtension(plugin));
                // If null, that means the file name is wrong.
                if (pluginInstance == null)
                {
                    throw new InvalidFileNameException("File name does not match Type name");
                }
                plugins.Add(pluginInstance.name, pluginInstance);
            }
        }

        [Serializable]
        public class InvalidFileNameException : Exception
        {
            public InvalidFileNameException() { }
            public InvalidFileNameException(string message) : base(message) { }
            public InvalidFileNameException(string message, Exception inner) : base(message, inner) { }
            protected InvalidFileNameException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context)
            { }
        }

    }
}
