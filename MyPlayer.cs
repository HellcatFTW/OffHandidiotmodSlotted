using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ID;
using CustomSlot.UI;
using CustomSlot;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace OffHandidiotmodSlotted
{
    public class MyPlayer : ModPlayer
    {
        private int slotIndex = 4; // Hotbar slot 5 (0-based index)
        private Item originalSelectedItem;
        private bool isUsingItem;
        public override void PreUpdate()
        {
            // Check if the right mouse button is held and the inventory is not open
            if (PlayerInput.Triggers.Current.MouseRight && !Main.playerInventory)
            {
                // Ensure we have a valid item in RMBSlot and selected slot
                if (MySlotUI.RMBSlot.Item.type != ItemID.None)                                           //  <----------- Player.selectedItem != ItemID.None) &&
                {
                    // Save the original selected item and initialize item usage
                    if (!isUsingItem)
                    {
                        originalSelectedItem = Player.inventory[Player.selectedItem];
                        Player.inventory[Player.selectedItem] = MySlotUI.RMBSlot.Item;



                        // Update item’s use time considering modifiers
                        Item item = Player.inventory[Player.selectedItem]; //                                                                  CHANGE THIS LINE TO RMBSLOT ITEM
                        int modifiedUseTime = (int)(item.useTime / Player.GetWeaponAttackSpeed(item)); // Adjust for attack speed

                        // Set animation and item time based on modified use time
                        Player.itemAnimation = modifiedUseTime;
                        Player.itemTime = modifiedUseTime;

                        isUsingItem = true;
                    }

                    // Simulate item use
                    if (Player.itemAnimation <= 0)
                    {
                        Player.controlUseItem = true;
                        Player.controlUseTile = false; // Ensure tile interactions do not interfere
                        Player.ItemCheck();

                        // Reset item animation and time
                        Item item = Player.inventory[Player.selectedItem];                                              // CHANGE THIS LINE TO RMBSLOT ITEM
                        int modifiedUseTime = (int)(item.useTime / Player.GetWeaponAttackSpeed(item));
                        Player.itemAnimation = modifiedUseTime;
                        Player.itemTime = modifiedUseTime;
                    }
                }
            }
            else
            {
                // Stop using the item and restore the original selected item
                if (isUsingItem)
                {
                    Player.controlUseItem = false;
                    Player.controlUseTile = false;
                    Player.inventory[Player.selectedItem] = originalSelectedItem;
                    isUsingItem = false;
                }
            }
        }
    }

    public class MyCustomSlotPlayer : ModPlayer
    {
        private PlayerData<Item> myCustomItem = new PlayerData<Item>("myitemtag", new Item());

        public override void OnEnterWorld()
        {
            // When the player enters the world, equip the correct items
            // SetItem() also fires the ItemChanged event by default
            MySlotUI.RMBSlot.SetItem(myCustomItem.Value);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Player.difficulty == PlayerDifficultyID.Creative || Player.difficulty == PlayerDifficultyID.SoftCore)
            {
                return;
            }

            // Ensure that the player drops items in the slot on death, if desired
            Player.QuickSpawnItem(Player.GetSource_Death(), myCustomItem.Value);

            // Remember that SetItem() fires the ItemChanged event, so if you have set up events then this will
            // update myCustomItem as desired
            MySlotUI.RMBSlot.SetItem(new Item());
        }

        public void ItemChanged(CustomItemSlot slot, ItemChangedEventArgs e)
        {
            // Here we update myCustomItem when MyNormalSlot fires ItemChanged
            myCustomItem.Value = e.NewItem.Clone();
        }

        // Ensure that the item is saved with the character
        public override void SaveData(TagCompound tag)
        {
            tag.Add(myCustomItem.Tag, ItemIO.Save(myCustomItem.Value));
        }

        // Ensure that the item loads with the character. This method is called on the character select screen
        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey(myCustomItem.Tag))
            {
                myCustomItem.Value = ItemIO.Load(tag.GetCompound(myCustomItem.Tag));
            }
        }
    }
}
