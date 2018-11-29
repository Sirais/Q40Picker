using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using System.Windows.Forms;

namespace Q40Picker
{
    internal class Q40PickerSettings : SettingsBase
    {

        [Menu("Hotkey for picking up gems")]
        public HotkeyNode Hotkey { get; set; }

        [Menu("Maximum Quality to Sell")]
        public RangeNode<int> MaxGemQuality { get; set; }

        [Menu("Maximum Level to Sell")]
        public RangeNode<int> MaxGemLevel { get; set; }

        [Menu("Extra Delay between Pickup Klicks")]
        public RangeNode<int> ExtraDelay { get; set; }


        public Q40PickerSettings()
        {
            //plugin 
            Enable = false;
            Hotkey = Keys.NumPad8;
            MaxGemQuality = new RangeNode<int> (18, 1, 18);
            MaxGemLevel = new RangeNode<int>(18, 1, 18);
            ExtraDelay = new RangeNode<int>(100, 1, 1000);
        }
    }
}
