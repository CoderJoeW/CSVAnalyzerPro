using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Analyzer_Pro.Core.PluginSystem {
    public class ExamplePlugin {
        public string Name {
            get {
                return "ExamplePlugin";
            }
        }

        public string Version {
            get {
                return "1.0.0";
            }
        }

        public string Description {
            get {
                return "This is an example class demonstrating how to make a plugin";
            }
        }
    }
}
