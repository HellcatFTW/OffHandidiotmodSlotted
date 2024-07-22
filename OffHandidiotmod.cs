using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;


namespace OffHandidiotmod
{
	public class OffHandidiotmod : Mod
	{
		public static UserInterface _myUserInterface;
		public static MySlotUI SlotUI;

		public override void Load()
		{
			// You can only display the UI to the local player -- prevent an error message!
			if (!Main.dedServ)
			{
				_myUserInterface = new UserInterface();
				SlotUI = new MySlotUI();

				SlotUI.Activate();
				_myUserInterface.SetState(SlotUI);
			}
		}

		public override void Unload()
		{
			// Ensure that you unload the UI's event handlers here
			SlotUI.Unload();
		}

		
	}
public class Activation : ModSystem
{
// Make sure the UI can draw
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			// This will draw on the same layer as the inventory
			int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

			if (inventoryLayer != -1)
			{
				layers.Insert(
					inventoryLayer,
					new LegacyGameInterfaceLayer("My Mod: My Slot UI", () =>
					{
						if (OffHandidiotmod.SlotUI.Visible)
						{
							OffHandidiotmod._myUserInterface.Draw(Main.spriteBatch, new GameTime());
						}

						return true;
					},
					InterfaceScaleType.UI));
			}
		}
		public static ModKeybind SwapKeybind { get; private set; }

        public override void Load() {
            // Registers a new keybind
            SwapKeybind = KeybindLoader.RegisterKeybind(Mod, "Swap Offhand", "T");
        }

        public override void Unload() {
            SwapKeybind = null;
        }
}
}