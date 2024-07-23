using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
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
		public static ModKeybind UseOffhandKeybind { get; private set; }

		public override void Load()
		{
			// Register new keybind
			SwapKeybind = KeybindLoader.RegisterKeybind(Mod, "Swap Offhand", "T");
			UseOffhandKeybind = KeybindLoader.RegisterKeybind(Mod, "Use Offhand Item", "Mouse2");
		}

		public override void Unload()
		{
			SwapKeybind = null;
			UseOffhandKeybind = null;
		}
	}
	public class OffHandConfig : ModConfig
	{
		// ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
		// ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviors
		public override ConfigScope Mode => ConfigScope.ClientSide; //it toggles a per-client message so i guess clientside

		// The things in brackets are known as "Attributes".

		// [Header("Items")] // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category. 
		[LabelKey("$Some.Key")] // A label is the text displayed next to the option. This should usually be a short description of what it does. By default all ModConfig fields and properties have an automatic label translation key, but modders can specify a specific translation key.
		[TooltipKey("$Some.Key")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option. Like with Label, a specific key can be provided.
		[DefaultValue(true)] // This sets the configs default value.
							 // [ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
		public bool ChatMessageToggle; // To see the implementation of this option, see ExampleWings.cs
	}
}