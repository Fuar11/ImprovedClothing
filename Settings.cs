using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModSettings;

namespace ImprovedClothing
{
    internal class CustomSettings : JsonModSettings
    {

        [Section("Clothing Placement Key Bindings")]

        [Name("Footwear Placement")]
        [Description("Click to set the keybinding for placing shoes, socks, etc....")]
        public KeyCode footwearKey = KeyCode.Keypad1;

        [Name("Gloves Placement")]
        [Description("Click to set the keybinding for placing gloves.")]
        public KeyCode glovesKey = KeyCode.Keypad2;

        [Name("Headwear Placement")]
        [Description("Click to set the keybinding for placing hats and scarves.")]
        public KeyCode hatKey = KeyCode.Keypad3;

        [Name("Coat/Jacket Placement")]
        [Description("Click to set the keybinding for placing any clothing on the body section, coats, jackets, sweaters, etc...")]
        public KeyCode coatKey = KeyCode.Keypad4;

        [Name("Pants Placement")]
        [Description("Click to set the keybinding for placing pants and underwear.")]
        public KeyCode pantsKey = KeyCode.Keypad5;

    }

    static class Settings
    {
        internal static CustomSettings settings;
        internal static void OnLoad()
        {
            settings = new CustomSettings();
            settings.AddToModSettings("Improved Clothing");
        }
    }
}
