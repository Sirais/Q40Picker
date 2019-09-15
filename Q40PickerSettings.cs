using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;
using System.Windows.Forms;

namespace Q40Picker
{
    internal class Q40PickerSettings : ISettings
    {

        [Menu("Hotkey for picking up gems")]
        public HotkeyNode Hotkey { get; set; }

        [Menu("Maximum Quality to Sell")]
        public RangeNode<int> MaxGemQuality { get; set; }

        [Menu("Maximum Level to Sell")]
        public RangeNode<int> MaxGemLevel { get; set; }

        [Menu("Extra Delay between Pickup Klicks")]
        public RangeNode<int> ExtraDelay { get; set; }

        [Menu("Use Flasks instead of Gems")]
        public ToggleNode UseFlask { get; set; }

        [Menu("Enable")]
        public ToggleNode Enable { get; set; }

        public Q40PickerSettings()
        {
            //plugin 
            Hotkey = Keys.NumPad8;
            MaxGemQuality = new RangeNode<int> (18, 1, 19);
            MaxGemLevel = new RangeNode<int>(18, 1, 19);
            ExtraDelay = new RangeNode<int>(100, 1, 1000);
            UseFlask = new ToggleNode(false);
            Enable = new ToggleNode(false);
        }
    }
}
