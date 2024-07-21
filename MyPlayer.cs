using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ID;

namespace OffHandidiotmodSlotted
{
    public class MyPlayer : ModPlayer
    {
        private int originalSelectedItem;
        private bool isUsingItem;

        public override void PreUpdate()
        {
            CustomSlotHandler customSlotHandler = Player.GetModPlayer<CustomSlotHandler>();
            
            // Check if the right mouse button is held and the inventory is not open
            if (PlayerInput.Triggers.Current.MouseRight && !Main.playerInventory)
            {
                // Ensure we have a valid item in the custom slot
                if (customSlotHandler.customSlotItem != ItemID.None)
                {
                    // Save the original selected item and initialize item usage
                    if (!isUsingItem)
                    {
                        originalSelectedItem = Player.selectedItem;
                        Player.selectedItem = customSlotHandler.customSlotItem;

                        // Update item’s use time considering modifiers
                        Item item = Player.inventory[Player.selectedItem];
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
                        Item item = Player.inventory[Player.selectedItem];
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
                    Player.selectedItem = originalSelectedItem;
                    isUsingItem = false;
                }
            }
        }
    }
}
