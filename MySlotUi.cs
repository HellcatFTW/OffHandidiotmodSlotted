using CustomSlot;
using CustomSlot.UI;
using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using System;
using System.Linq;
using Terraria.GameContent.UI;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections;

namespace OffHandidiotmod
{
    public class MySlotUI : UIState
    {
        public static bool CalamityEnabled;
        public static bool QoTEnabled;
        public static Mod Calamity;
        public class SomethingSlot : CustomItemSlot
        {

            public int cooldownCount;
            private double emptyBuffAmount = 0;
            private int rowGap = 0;
            private const int rowSize = 43;
            private int buffRows = 0;
            private int goblinOffset = 0;
            private const int defaultPosYHUD = 79;
            private const int defaultPosXHUD = 25;
            private const int defaultPosYInventory = 260;
            private const int defaultPosXInventory = 20;
            private const int PosXIgnoreThreshold = 450;
            private const int journeyOffset = 50;
            private int calamityOffsetY = 0;
            private int calamityOffsetX = 0;
            private int qotOffsetY = 0;
            private int qotOffsetX = 0;
            private int currentDynamicOffset;
            private int PosYInventory { get => ModContent.GetInstance<OffHandConfig>().SlotPositionYInventory; }
            private int PosYHUD { get => ModContent.GetInstance<OffHandConfig>().SlotPositionYHUD; }
            private int PosXInventory { get => ModContent.GetInstance<OffHandConfig>().SlotPositionXInventory; }
            private int PosXHUD { get => ModContent.GetInstance<OffHandConfig>().SlotPositionXHUD; }
            public SomethingSlot() : base(ItemSlot.Context.InventoryItem, 0.85f)
            {
                IsValidItem = item => item.type > ItemID.None && !ItemID.Sets.Torches[item.type] && !ItemID.Sets.Glowsticks[item.type];
            }

            public int? getCalamityCooldowns()
            {
                if (Calamity == null)
                {
                    return null;
                }
                try
                {
                    ModPlayer calamityPlayerInstance = Main.LocalPlayer.GetModPlayer(Activation.calamityPlayerTemplate);

                    IDictionary cooldowns = (IDictionary)Activation.getCooldowns.GetValue(calamityPlayerInstance);
                    
                    return cooldowns.Count;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                string SlotHoverText = Language.GetText("Mods.OffHandidiotmod.SlotHoverText").Value;

                emptyBuffAmount = Main.LocalPlayer.buffTime.Count(0); // includes modded buffs too :D

                buffRows = (int)Math.Ceiling((Main.LocalPlayer.buffTime.Length - emptyBuffAmount) / 11); // divide buffs by 11 which is amount of buffs in a row

                if (buffRows > 1)
                {
                    rowGap = 4;
                }
                else
                {
                    rowGap = 0;
                }

                if (getCalamityCooldowns() != null && getCalamityCooldowns() != 0) // Calamitymod cooldown timers offset
                {
                    calamityOffsetY = 57;
                    calamityOffsetX = 7;
                }
                else
                {
                    calamityOffsetY = 0;
                    calamityOffsetX = 0;
                }

                if (QoTEnabled)
                {
                    qotOffsetY = 50;
                    qotOffsetX = 48;
                }
                else
                {
                    qotOffsetY = 0;
                    qotOffsetX = 0;
                }

                if (Main.InReforgeMenu) // goblin tinkerer reforge slot offset
                {
                    goblinOffset = 60;
                }
                else
                {
                    goblinOffset = 0;
                }

                currentDynamicOffset = (buffRows * rowSize) + (rowGap * buffRows) + calamityOffsetY; // buffs + calamity offset for HUD





                // Sets position for inventory
                if (Main.playerInventory)
                {
                    HoverText = SlotHoverText;
                    ItemRarity = ItemRarityID.White;


                    // Sets position for inventory
                    if (Main.LocalPlayer.difficulty != 3) // Regular modes
                    {
                        if (Main.npcShop == 0) // not in a shop
                        {
                            if (Main.InReforgeMenu) // is in reforge menu specifically
                            {
                                RMBSlot.Left.Set(defaultPosXInventory, 0);
                                RMBSlot.Top.Set(defaultPosYInventory + goblinOffset, 0);
                            }
                            else // is in regular inventory
                            {
                                RMBSlot.Left.Set(PosXInventory + qotOffsetX, 0);
                                RMBSlot.Top.Set(PosYInventory + qotOffsetY, 0);
                            }
                        }
                        else // is in a shop
                        {
                            RMBSlot.Left.Set(defaultPosXInventory, 0);
                            RMBSlot.Top.Set(defaultPosYInventory, 0);
                        }
                    }
                    else // In journey mode
                    {
                        if (Main.npcShop == 0 && !Main.InReforgeMenu) // regular inventory
                        {
                            RMBSlot.Left.Set(PosXInventory + journeyOffset, 0); // 50 pixel offset to right for journey mode power menu thing
                            RMBSlot.Top.Set(PosYInventory + qotOffsetY, 0);
                        }
                        else // shop is open or player reforging
                        {
                            if (Main.InReforgeMenu) //reforging
                            {
                                RMBSlot.Left.Set(defaultPosXInventory, 0);
                                RMBSlot.Top.Set(defaultPosYInventory + goblinOffset, 0); // no QoT offset because the menu disappears in tinkerer shop
                            }
                            else // shop is open
                            {
                                RMBSlot.Left.Set(defaultPosXInventory, 0);
                                RMBSlot.Top.Set(defaultPosYInventory, 0);
                            }
                        }
                    }
                }







                // Sets position for HUD
                else
                {

                    // when player is hovering over item in hotbar state, and hotbar isnt locked, and there's an item in the slot
                    if (!Main.LocalPlayer.ItemAnimationActive && !Main.LocalPlayer.hbLocked && RMBSlot.item.type > ItemID.None)
                    {
                        HoverText = RMBSlot.Item.AffixName();
                        ItemRarity = RMBSlot.item.rare;
                        if (RMBSlot.Item.stack > 1)
                            HoverText = HoverText + " (" + RMBSlot.Item.stack + ")";
                    }
                    else
                    {
                        HoverText = "";
                        ItemRarity = null;
                    }



                    // Sets position for HUD
                    if (PosYHUD == defaultPosYHUD && PosXHUD == defaultPosXHUD) // HUD is default position, add dynamic offset
                    {
                        RMBSlot.Left.Set(defaultPosXHUD + calamityOffsetX, 0);
                        RMBSlot.Top.Set(defaultPosYHUD + currentDynamicOffset, 0);
                    }
                    else
                    {
                        if (PosXHUD >= PosXIgnoreThreshold) // Special case that slot is set to right of hotbar, no need to lower it with buffs.
                        {
                            RMBSlot.Left.Set(PosXHUD + calamityOffsetX, 0); // leave calamityoffset there for consistent user experience
                            RMBSlot.Top.Set(PosYHUD, 0);
                        }
                        else // X is not at ignore threshold and positions are not default
                        {
                            if (PosYHUD > currentDynamicOffset + defaultPosYHUD) // Config position is outside current active buffs area
                            {
                                RMBSlot.Left.Set(PosXHUD + calamityOffsetX, 0);
                                RMBSlot.Top.Set(PosYHUD, 0);
                            }
                            else // config position is IN current active buffs area, 
                            {
                                RMBSlot.Left.Set(PosXHUD + calamityOffsetX, 0);
                                RMBSlot.Top.Set(defaultPosYHUD + currentDynamicOffset, 0);
                            }
                        }
                    }
                }





                base.DrawSelf(spriteBatch);
            }// end of drawself override
        }// end of class

        public static SomethingSlot RMBSlot;

        public bool Visible = true;
        //{
        //    get => Main.playerInventory; // how do you display your slot?
        //}
        public override void OnInitialize()
        {
            RMBSlot = new SomethingSlot();

            // You can set these once or change them in DrawSelf()

            // Don't forget to add them to the UIState!
            Append(RMBSlot);


            // If you're going to hook into CustomItemSlot events, put them here, then unload them during MyMod.Unload()
            RMBSlot.ItemChanged += ItemChanged;
        }

        private void ItemChanged(CustomItemSlot slot, ItemChangedEventArgs e)
        {
            // It's usually best to "encapsulate" data: that is, let the class that owns it handle it, while calling only
            // public functions
            Main.LocalPlayer.GetModPlayer<MyCustomSlotPlayer>().ItemChanged(slot, e);
        }

        // Unload the class by removing its event handlers
        internal void Unload()
        {
            RMBSlot.ItemChanged -= ItemChanged;
        }
    }
}