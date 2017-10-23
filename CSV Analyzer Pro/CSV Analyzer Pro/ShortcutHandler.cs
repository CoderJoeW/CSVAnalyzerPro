using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSV_Analyzer_Pro { 
    class ShortcutHandler {
        public static ShortcutHandler Instance { set; get; }
        public enum shortcuts {
            NoShortcut, NewWindow, Open,
            Save,NewPluginStoreTab,
        };
        Dictionary<string, bool> keyStates = new Dictionary<string, bool>();

        public shortcuts shortcut = shortcuts.NoShortcut;

        public ShortcutHandler() {
            Instance = this;
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
            }else if(CheckKeyDown(Keys.ControlKey.ToString()) && CheckKeyDown(Keys.S.ToString())){
                shortcut = shortcuts.Save;
            }else if(CheckKeyDown(Keys.ControlKey.ToString()) && CheckKeyDown(Keys.O.ToString())) {
                shortcut = shortcuts.Open;
            }else if(CheckKeyDown(Keys.Alt.ToString()) && CheckKeyDown(Keys.N.ToString())) {
                shortcut = shortcuts.NewPluginStoreTab;
            }

            return shortcut;
        }

    }
}
