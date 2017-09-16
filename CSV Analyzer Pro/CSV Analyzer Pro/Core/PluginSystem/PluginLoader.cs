using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace CSV_Analyzer_Pro.Core.PluginSystem {
    public class PluginLoader {
        public static List<IPlugin> Plugins { set; get; }

        public void LoadPlugins() {
            Plugins = new List<IPlugin>();

            //Load from plugins directory
            if (Directory.Exists(Constants.PluginFolder)) {
                string[] files = Directory.GetFiles(Constants.PluginFolder);
                foreach(string file in files) {
                    if (file.EndsWith(".dll")) {
                        Assembly.LoadFile(Path.GetFullPath(file));
                    }
                }
            }

            Type interfaceType = typeof(IPlugin);

            Type[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass).ToArray();

            foreach(Type type in types) {
                Plugins.Add((IPlugin)Activator.CreateInstance(type));
            }
        }
    }
}
