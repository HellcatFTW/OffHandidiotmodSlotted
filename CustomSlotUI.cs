using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;

namespace OffHandidiotmodSlotted
{
    public class CustomSlotUI : UIState
    {
        private UIImageButton customSlotButton;
        private const int SlotSize = 50; // Size of the slot

        public override void OnInitialize()
        {
            // Adjust position: 20 pixels from the left side and 1/3rd of the screen height vertically
            int slotX = 20; // 20 pixels from the left
            int slotY = Main.screenHeight / 3 - SlotSize / 2; // 1/3 of the screen height

            customSlotButton = new UIImageButton(ModContent.Request<Texture2D>("OffHandidiotmodSlotted/Assets/UI/CustomSlotImage"))
            {
                Left = { Pixels = slotX },
                Top = { Pixels = slotY },
                Width = { Pixels = SlotSize },
                Height = { Pixels = SlotSize }
            };

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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
