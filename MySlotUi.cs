using CustomSlot;
using CustomSlot.UI;
using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace OffHandidiotmod
{
    public class MySlotUI : UIState
    {
        public class SomethingSlot : CustomItemSlot
        {
            public SomethingSlot() : base(ItemSlot.Context.InventoryItem, 0.85f)
            {
                IsValidItem = item => item.type > ItemID.None && !ItemID.Sets.Torches[item.type] && !ItemID.Sets.Glowsticks[item.type];
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                string SlotHoverText = Language.GetText("Mods.OffHandidiotmod.SlotHoverText").Value;
                HoverText = SlotHoverText;
                RMBSlot.Left.Set(20, 0);
                RMBSlot.Top.Set(260, 0);
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