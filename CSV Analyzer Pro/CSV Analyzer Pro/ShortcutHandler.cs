using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSV_Analyzer_Pro { 
    class ShortcutHandler {

        public enum shortcuts { NoShortcut, NewWindow };
        Dictionary<string, bool> keyStates = new Dictionary<string, bool>();

        public ShortcutHandler() {

        }

        public bool CheckKeyDown(string key) {

            bool keyDown;
            bool keyInDict;

            keyInDict = keyStates.TryGetValue(key, out keyDown);

            if (!keyInDict) {
                keyDown = false;  // Assuming key is up if it has not previously been set as down
                keyStates.Add(key, keyDown);
            }

            return keyDown;

        }

        public void ReportKeyUp(string key) {

            bool keyInDict;

            keyInDict = keyStates.ContainsKey(key);

            if (!keyInDict) {
                keyStates.Add(key, false);
            }
            else {
                keyStates[key] = false;
            }

        }

        public void ReportKeyDown(string key) {

            bool keyInDict;

            keyInDict = keyStates.ContainsKey(key);

            if (!keyInDict) {
                keyStates.Add(key, true);
            }
            else {
                keyStates[key] = true;
            }

        }

        public shortcuts CheckShortcuts() {

            shortcuts shortcut = shortcuts.NoShortcut;

            if (CheckKeyDown(Keys.ControlKey.ToString()) && CheckKeyDown(Keys.N.ToString())) {
                shortcut = shortcuts.NewWindow;
            }

            return shortcut;

        }

    }
}
