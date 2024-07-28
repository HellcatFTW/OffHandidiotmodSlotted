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

namespace OffHandidiotmod
{
    public class MySlotUI : UIState
    {
        public class SomethingSlot : CustomItemSlot
        {
            private double emptybuffamount;
            private int rowgap;
            private int buffrows;
            private int goblinOffset;
            private const int journeyOffset = 50;
            private int PosYInventory {get => ModContent.GetInstance<OffHandConfig>().SlotPositionYInventory;}
            private int PosYHUD {get => ModContent.GetInstance<OffHandConfig>().SlotPositionYHUD;}
            private int PosXInventory {get => ModContent.GetInstance<OffHandConfig>().SlotPositionXInventory;}
            private int PosXHUD {get => ModContent.GetInstance<OffHandConfig>().SlotPositionXHUD;}
            public SomethingSlot() : base(ItemSlot.Context.InventoryItem, 0.85f)
            {
                IsValidItem = item => item.type > ItemID.None && !ItemID.Sets.Torches[item.type] && !ItemID.Sets.Glowsticks[item.type];
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                string SlotHoverText = Language.GetText("Mods.OffHandidiotmod.SlotHoverText").Value;
                HoverText = SlotHoverText;
                if (Main.playerInventory) // Inventory open
                {
                    if(Main.InReforgeMenu)
                    {
                        goblinOffset = 60;
                    }
                    else
                    {
                        goblinOffset = 0;
                    }
                    
                    if (Main.LocalPlayer.difficulty != 3) // In all modes but journey mode
                    {
                        RMBSlot.Left.Set(PosXInventory, 0);
                        RMBSlot.Top.Set(PosYInventory+goblinOffset, 0);
                    }
                    else // In journey mode
                    {
                        RMBSlot.Left.Set(PosXInventory+journeyOffset, 0); // 50 pixel offset to right for journey mode power menu thing
                        RMBSlot.Top.Set(PosYInventory, 0);
                    }
                }
                else // Inventory closed
                {
                    emptybuffamount = Main.LocalPlayer.buffTime.Count(0);

                    buffrows = (int)Math.Ceiling(
                        (Main.LocalPlayer.buffTime.Length - emptybuffamount) / 11); // divide buffs by 11 which is amount of buffs in a row 
                    if (buffrows > 1)
                    {
                        rowgap = 4;
                    }
                    else
                    {
                        rowgap = 0;
                    }
                    RMBSlot.Left.Set(PosXHUD, 0);
                    RMBSlot.Top.Set(PosYHUD + (buffrows * 43) + (rowgap * buffrows), 0);
                }



                base.DrawSelf(spriteBatch);
            }
        }

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