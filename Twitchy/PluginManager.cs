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
        public static PluginManager instance = new PluginManager();

        public Dictionary<string, Plugin> plugins = new Dictionary<string, Plugin>();
        
        public void initializePages()
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
                Assembly assembly = (Assembly.LoadFile(plugin));
                Plugin pluginInstance = (Plugin)assembly.CreateInstance(Path.GetFileNameWithoutExtension(plugin));
                instance.plugins.Add(pluginInstance.name, pluginInstance);
            }
        }
    }
}
