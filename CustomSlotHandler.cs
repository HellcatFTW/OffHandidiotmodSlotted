using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace OffHandidiotmodSlotted
{
    public class CustomSlotHandler : ModSystem
    {
        private UserInterface customSlotInterface;
        private CustomSlotUI customSlotUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                customSlotUI = new CustomSlotUI();
                customSlotUI.Activate();
                customSlotInterface = new UserInterface();
                customSlotInterface.SetState(customSlotUI);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (customSlotInterface?.CurrentState != null)
            {
                customSlotInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "OffHandidiotmodSlotted: Custom Slot UI",
                    delegate
                    {
                        customSlotInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
