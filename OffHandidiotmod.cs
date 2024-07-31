using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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
		public static ModPlayer calamityPlayerTemplate;
		public static FieldInfo getCooldowns;
		public override void PostSetupContent()
		{

			if (ModLoader.TryGetMod("CalamityMod", out Mod Calamity)) // Calamity mod
			{
				MySlotUI.Calamity = Calamity;

				calamityPlayerTemplate = ModContent.Find<ModPlayer>("CalamityMod/CalamityPlayer");

				getCooldowns = calamityPlayerTemplate.GetType().GetField("cooldowns");
			}
			else
			{
				MySlotUI.Calamity = null;
			}


			if (ModLoader.TryGetMod("ImproveGame", out _)) // Quality of terraria mod that adds stupid ass 20 trash slots
			{
				MySlotUI.QoTEnabled = true;
			}
			else
			{
				MySlotUI.QoTEnabled = false;
			}


		}

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
			SwapKeybind = KeybindLoader.RegisterKeybind(Mod, "Swap Offhand", "Q");
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
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[LabelKey("$Mods.OffHandidiotmod.Configs.OffHandConfig.Label")]
		[DefaultValue(true)]
		public bool ChatMessageToggle;

		[Header("$Mods.OffHandidiotmod.Configs.OffHandConfig.HUDHeader")]
		[TooltipKey("$Mods.OffHandidiotmod.Configs.OffHandConfig.SlotPositionXHUD.Tooltip")]
		[DefaultValue(25)]
		[Range(0, 1920)]
		public int SlotPositionXHUD;

		[TooltipKey("$Mods.OffHandidiotmod.Configs.OffHandConfig.SlotPositionYHUD.Tooltip")]
		[DefaultValue(79)]
		[Range(0, 1080)]
		public int SlotPositionYHUD;

		[Header("$Mods.OffHandidiotmod.Configs.OffHandConfig.InventoryHeader")]
		[TooltipKey("$Mods.OffHandidiotmod.Configs.OffHandConfig.SlotPositionXInventory.Tooltip")]
		[DefaultValue(20)]
		[Range(0, 1920)]
		public int SlotPositionXInventory;

		[TooltipKey("$Mods.OffHandidiotmod.Configs.OffHandConfig.SlotPositionYInventory.Tooltip")]
		[DefaultValue(260)]
		[Range(0, 1080)]
		public int SlotPositionYInventory;
	}
}