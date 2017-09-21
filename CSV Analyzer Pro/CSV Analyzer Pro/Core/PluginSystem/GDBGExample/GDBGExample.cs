using CSV_Analyzer_Pro.Core.PluginSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Windows.Forms.Grid;

namespace CSV_Analyzer_Pro.Core.PluginSystem.GDBGExample {
    public class GDBGExample: IPlugin {
        public string Name {
            get {
                return "GridDataBoundGrid Example";
            }
        }

        public string Version {
            get {
                return "1.0.0";
            }
        }

        public string TargetVersion {
            get {
                return "0.0.3";
            }
        }

        public string Description {
            get {
                return "Shows how to add access the GridDataBoundGrid and change the theme";
            }
        }

        public string TargetFramework {
            get {
                return "GridDataBoundGrid";
            }
        }

        void IPlugin.Action(GridDataBoundGrid dbg) {
            dbg.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.SystemTheme;
        }
    }
}
