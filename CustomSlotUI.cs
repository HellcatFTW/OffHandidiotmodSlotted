using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace OffHandidiotmodSlotted
{
    public class CustomSlotUI : UIState
    {
        private UIImageButton customSlotButton;
        private const int SlotX = 10; // Position X
        private const int SlotY = 10; // Position Y
        private const int SlotSize = 50; // Size of the slot

        public override void OnInitialize()
        {
            customSlotButton = new UIImageButton(ModContent.Request<Texture2D>("OffHandidiotmodSlotted/Assets/UI/CustomSlotImage"))
            {
                Left = { Pixels = SlotX },
                Top = { Pixels = SlotY },
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
