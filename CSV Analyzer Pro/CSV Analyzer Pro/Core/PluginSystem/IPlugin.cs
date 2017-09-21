using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Windows.Forms.Grid;

namespace CSV_Analyzer_Pro.Core.PluginSystem {
    public interface IPlugin {
        string Name { get; }
        string Version { get; }
        string TargetVersion { get; }
        string Description { get; }
        string TargetFramework { get; }
        void Action(GridDataBoundGrid dbg);
    }
}
