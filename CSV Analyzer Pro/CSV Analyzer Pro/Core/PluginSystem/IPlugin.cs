using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Analyzer_Pro.Core.PluginSystem {
    public interface IPlugin {
        string Name { get; }
        string Version { get; }
        string Description { get; }
    }
}
