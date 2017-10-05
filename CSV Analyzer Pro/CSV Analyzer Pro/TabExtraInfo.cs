using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Analyzer_Pro
{
    public class TabExtraInfo {

        private int associatedTabIndex; // Will not be updated as tabs are open and closed, be wary of relying on it
        private string assocaitedFileName;
        private bool hasUnsavedChanges;

        public TabExtraInfo(int index, string fileName) {
            associatedTabIndex = index;
            assocaitedFileName = fileName;
            hasUnsavedChanges = false;
        }

        public void SetAssociatedFileName(string filename) {
            assocaitedFileName = filename;
        }

        public string GetAssocaitedFileName() {
            return assocaitedFileName;
        }

        public int GetAssociatedTabIndex() {
            return associatedTabIndex;
        }

        public void SetHasUnsavedChanges(bool yn) {
            hasUnsavedChanges = yn;
        }

        public bool QueryHasUnsavedChanges() {
            return hasUnsavedChanges;
        }

    }
}
