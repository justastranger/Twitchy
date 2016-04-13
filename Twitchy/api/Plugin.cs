using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace Twitchy.api
{
    public abstract class Plugin
    {
        // The name of the plugin
        abstract public string name { get; set; }
        // This will be the plugin's local copy of its config section
        // Don't access any config but this
        abstract public JObject config { get; set; }

        // Return null if something is preventing you from doing this, the plugin will
        // still initialize its config (allowing people to set up the plugin) but the
        // null will be silently discarded
        abstract public TabPage getPage();
    }
}
