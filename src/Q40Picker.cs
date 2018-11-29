using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PoeHUD.Controllers;
using PoeHUD.Models;
using PoeHUD.Models.Interfaces;
using PoeHUD.Plugins;
using PoeHUD.Poe.Elements;
using PoeHUD.Poe;
using PoeHUD.Poe.Components;
using PoeHUD.Poe.RemoteMemoryObjects;
using SharpDX;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ImGuiNET;
using ImVector2 = System.Numerics.Vector2;
using ImVector4 = System.Numerics.Vector4;
using Vector4 = SharpDX.Vector4;
using Druzil.Poe.Libs;
using PoeHUD.Framework;
using PoeHUD.Framework.Helpers;



namespace Q40Picker
{
    internal class Q40Picker : BaseSettingsPlugin<Q40PickerSettings>
    {
        public override void Render()
        {
            base.Render();
            if (!Settings.Enable.Value) // Plugin enabled = 
                return; // no, do nothing

            if (!KeyboardHelper.IsKeyToggled(Settings.Hotkey.Value)) // Hotkey Pressed ? 
                return; // No key pressed just leave

            LogMessage($"Picker: Hotkey ({Settings.Hotkey.Value.ToString()}) toggled, Running Q40 Picker", 1);

            if (!GameController.Game.IngameState.ServerData.StashPanel.IsVisible) // ch: No Gems without open Stash
            {
                LogMessage($"No Open Stash -> leaving ", 1);
                KeyboardHelper.KeyPress(Settings.Hotkey.Value);
                return;
            }

            List<setData> gems = getQualityGems();
            LogMessage($"Picker: found  {gems.Count} Quality Items in open stash.", 1);
            if (gems == null || gems.Count == 0)
            {
                LogMessage("No Quality gems found ", 1);
                KeyboardHelper.KeyPress(Settings.Hotkey.Value);
                return;
            }

            SetFinder Sets = new SetFinder(gems, 40); 


            if (Sets.BestSet == null)
            {
                LogMessage("Added Quality is not 40", 1);
                KeyboardHelper.KeyPress(Settings.Hotkey.Value);
                return;
            }


            pickup(Sets);

            KeyboardHelper.KeyPress(Settings.Hotkey.Value); // send the hotkey back to the system to turn off the Work

        }


        // Displays found set as  Logmessage. Just for debugging 
        private void displaySet(SetFinder Sets)
        {
            LogMessage($"V5 :found set for Q40 contains {Sets.BestSet.Values.Count} Gems", 10);

            int i = 1;
            foreach (QualityGem g in Sets.BestSet.Values)
            {
                LogMessage($"{i} - Q{g.getValue()} X{g.Gem.InventPosX} - Y{g.Gem.InventPosY}", 10);
                i++;
            }
        }

        // time to Pickup found Items into main inventory
        private void pickup(SetFinder Sets)
        {
            foreach (QualityGem g in Sets.BestSet.Values)
            {
                RectangleF itmPos = g.Gem.GetClientRect();
                KeyboardHelper.KeyDown(System.Windows.Forms.Keys.LControlKey);
                Thread.Sleep((int)GameController.Game.IngameState.CurLatency);
                Mouse.SetCursorPosAndLeftClick(RandomizedCenterPoint(itmPos), GameController.Window.GetWindowRectangle().TopLeft);
                Thread.Sleep((int)GameController.Game.IngameState.CurLatency);
                KeyboardHelper.KeyUp(System.Windows.Forms.Keys.LControlKey);
                Thread.Sleep((int)GameController.Game.IngameState.CurLatency);
                Thread.Sleep(Settings.ExtraDelay);
            }
        }


        // Randomitze clicking Point for Items
        private static Vector2 RandomizedCenterPoint(RectangleF rec)
        {
            var randomized = rec.Center;
            var xOffsetMin = (int)(-1 * rec.Width / 2) + 2;
            var xOffsetMax = (int)(rec.Width / 2) - 2;
            var yOffsetMin = (int)(-1 * rec.Height / 2) + 2;
            var yOffsetMax = (int)(rec.Height / 2) - 2;
            var random = new Random();

            randomized.X += random.Next(xOffsetMin, xOffsetMax);
            randomized.Y += random.Next(yOffsetMin, yOffsetMax);

            return randomized;
        }




        public override void Initialise()
        {
            PluginName = "Auto Q40Picker";
        }

        /// <summary>
        /// Fetches a list of Quality gems from current open inventory
        /// </summary>
        /// <returns></returns>
        private List<setData> getQualityGems()
        {
            List<setData> res = new List<setData>();
            var stashPanel = GameController.Game.IngameState.ServerData.StashPanel;
            if (!stashPanel.IsVisible)
                return null;
            var visibleStash = stashPanel.VisibleStash;
            if (visibleStash == null)
                return null;
            List<NormalInventoryItem> inventoryItems = GameController.Game.IngameState.ServerData.StashPanel.VisibleStash.VisibleInventoryItems;
            foreach (NormalInventoryItem item in inventoryItems)
            {
                var baseItemType = BasePlugin.API.GameController.Files.BaseItemTypes.Translate(item.Item.Path);
                if (baseItemType.ClassName.Contains("Skill Gem"))
                {
                    int Quality = item.Item.GetComponent<Quality>().ItemQuality;
                    if (Quality > 0)
                        if (Quality<= Settings.MaxGemQuality)
                            res.Add(new QualityGem( item,Quality));

                }
            }
            return res;
        }

    }

    /// <summary>
    /// Collectionobject for subsetSum Quality 40 !
    /// </summary>
    public class QualityGem : setData
    {
        public NormalInventoryItem Gem { get; set; }
        public int Quality { get; set; }
        public QualityGem (NormalInventoryItem gem,int quality)
        {
            Gem = gem;
            Quality = quality;
        }

        public int getValue()
        {
            return Quality; // Gem.GetComponent<Quality>().ItemQuality;
        }

        public override string ToString()
        {
            return Gem.Item.ToString();
        }

    }




}

