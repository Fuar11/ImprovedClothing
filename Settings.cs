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
        [Section("New Features")]

        [Name("Enable Struggle Wetness")]
        [Description("Animal struggles in the snow will cause outer layer clothing to get wet.\n" +
            "Ranges from ~15% to ~60% based on gear type.\n\nChoose [No] for vanilla behavior.")]
        public bool enableWetnessStruggle = true;

        [Name("Enable Condition-based Wetness")]
        [Description("Global wetness gain (including from vanilla weather) will be increased as clothing gets lower condition.\n\n" +
            "This is strongly multiplicative:\n > 50% condition = 2x wetness\n > 20% condition = 5x wetness\n\nChoose [No] for vanilla behavior.")]
        public bool enableWetnessCondition = true;

        [Name("Enable Footwear Wetness")]
        [Description("Walking on snow/ice will gradually cause footwear to get wet, up to a limit based on missing condition.\n\n" +
            "Traversing inclines can quickly drench socks with overflow snow, if footwear is inadequate (boots are better than sneakers).\n\n" +
            "Snow will slowly seep into socks if footwear is in poor condition (below 65% by default).\n\nChoose [No] for vanilla behavior.")]
        public bool enableWetnessFootstep = true;

        [Section("Footwear Wetness Tweaks")]

        [Name("Wetness Rate Multiplier")]
        [Description("Percentage modifier to wetness when walking on snow/ice.\n\n" +
            "Reduce this if you feel boots are getting wet too quickly when walking outside\n\n0% = disabled\n200% = doubled\n\nMod default: 100%")]
        [Slider(0f, 200f, 41)]
        public float footstepWetnessMult = 100f;

        [Name("Wetness Rate Multiplier - Ice")]
        [Description("Percentage modifier to wetness when walking on flat ice compared to snow.\n\n" +
            "0% = no wetness gain on ice\n100% = same wetness gain as snow\n\nMod default: 65%")]
        [Slider(0f, 100f, 21)]
        public float footstepWetnessMultIce = 65f;

        [Name("Wetness Limit Multiplier")]
        [Description("Percentage modifier to the max wetness outer footwear can reach from walking in snow.\n" +
            "Limit is based on footwear type, and increases from poor condition.\n\n0% = disabled\n200% = doubled\n\n" +
            "Mod defaults: 100%:\n > Shoes (full cond): around 40% wetness limit.\n > Boots (full cond): around 20% wetness limit")]
        [Slider(0f, 200f, 41)]
        public float footstepWetnessCapMult = 100f;

        [Name("Wetness Limit Condition Threshold")]
        [Description("Footwear below this condition will suffer increased wetness limit from walking in snow.\n\n" +
            "For example, Slider at 70%:\n > Gear above 70% cond = base wetness limit\n > Gear at 20% cond = 2x wetness limit\n\n" +
            "Mod default: 100%\n > Gear at 100% cond = base wetness limit\n > Gear at 50% cond = 2x wetness limit")]
        [Slider(0f, 100f, 21)]
        public float footstepWetnessCapThreshold = 100f;

        [Name("Sock Wetness Multiplier - Overflow")]
        [Description("Percentage modifier to wetness on socks when snow overflows.\n\n" +
            "Overflow is based on footwear type, and terrain incline. Inadequate shoes often overflow, but proper boots rarely do.\n\n" +
            "0% = disabled\n200% = doubled\n\nMod default: 100%")]
        [Slider(0f, 200f, 41)]
        public float footstepWetnessMultSockOverflow = 100f;

        [Name("Sock Wetness Multiplier - Seepage")]
        [Description("Percentage modifier to wetness on socks when snow seeps through poor condition footwear.\n\n" +
            "Seep amount is based on missing condition of footwear.\n\n0% = disabled\n200% = doubled\n\nMod default: 100%")]
        [Slider(0f, 200f, 41)]
        public float footstepWetnessMultSockSeep = 100f;

        [Name("Seepage Condition Threshold")]
        [Description("Footwear must be below this condition for snow to seep through to socks.\n\n" +
            "For example, Slider at 70%:\n > Footwear above 70% cond = no seepage\n > Footwear at 70% cond = base seepage\n" +
            " > Footwear at 20% cond = 2x seepage\n\nMod default: 65%")]
        [Slider(0f, 100f, 21)]
        public float footstepWetnessSeepThreshold = 65f;

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
