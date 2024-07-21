using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using Terraria.GameContent.UI.Elements;

namespace OffHandidiotmodSlotted
{
    public class CustomSlotUI : UIState
    {
        private UIImageButton customSlotButton;
        private const int SlotX = 20; // Adjusted X position
        private const int SlotY = 300; // Adjusted Y position
        private const int SlotSize = 50; // Size of the slot

        public override void OnInitialize()
        {
            var texture = ModContent.Request<Texture2D>("OffHandidiotmodSlotted/Assets/UI/CustomSlotImage");

            customSlotButton = new UIImageButton(texture)
            {
                Left = { Pixels = SlotX },
                Top = { Pixels = SlotY },
                Width = { Pixels = SlotSize },
                Height = { Pixels = SlotSize }
            };

            // Attach the event handler for left click
            customSlotButton.OnLeftClick += OnSlotClick;

            Append(customSlotButton);
        }

        private void OnSlotClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // Open inventory or custom logic to select item
            Main.playerInventory = true;

            // Logic to set the item in the custom slot
            MyPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
            if (Main.mouseItem.type != ItemID.None)
            {
                modPlayer.customSlotItem = Main.mouseItem.Clone();
                Main.mouseItem.TurnToAir();
            }
            else
            {
                // Handle case where slot is empty, remove item from slot
                if (modPlayer.customSlotItem.type != ItemID.None)
                {
                    Main.mouseItem = modPlayer.customSlotItem.Clone();
                    modPlayer.customSlotItem.TurnToAir();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
