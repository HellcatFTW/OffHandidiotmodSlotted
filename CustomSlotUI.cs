using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;  // For ItemID.None
using ReLogic.Content;  // For Asset<Texture2D>

namespace OffHandidiotmodSlotted
{
    public class CustomSlotUI : UIState
    {
        private UIElement customSlot;
        private const int SlotX = 20; // Position X
        private const int SlotY = 300; // Position Y
        private const int SlotSize = 50; // Size of the slot
        private Item customSlotItem = new Item();

        public override void OnInitialize()
        {
            customSlot = new UIElement
            {
                Width = { Pixels = SlotSize },
                Height = { Pixels = SlotSize }
            };

            customSlot.Left.Set(SlotX, 0f);
            customSlot.Top.Set(SlotY, 0f);
            customSlot.OnLeftClick += OnSlotClick;

            Append(customSlot);
        }

        private void OnSlotClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.playerInventory)
            {
                if (Main.mouseItem.IsAir)
                {
                    // Move item from slot to cursor
                    Main.mouseItem = customSlotItem.Clone();
                    customSlotItem.TurnToAir();
                }
                else if (Main.mouseItem.type != ItemID.None)
                {
                    // Move item from cursor to slot
                    customSlotItem = Main.mouseItem.Clone();
                    Main.mouseItem.TurnToAir();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Draw the slot texture manually
            var texture = ModContent.Request<Texture2D>("OffHandidiotmodSlotted/Assets/UI/CustomSlotImage").Value;
            spriteBatch.Draw(texture, new Rectangle(SlotX, SlotY, SlotSize, SlotSize), Color.White);
        }
    }
}
