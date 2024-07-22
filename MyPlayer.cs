using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ID;
using CustomSlot.UI;
using CustomSlot;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using System.Security.Cryptography.X509Certificates;

namespace OffHandidiotmod
{
    public class MyPlayer : ModPlayer
    {
        private Item originalSelectedItem;
        private bool isUsingItem;
        public override void PreUpdate()
        {
            // Check if the right mouse button is held and the inventory is not open (to preserve RMB functionality on treasure bags etc)
            if (PlayerInput.Triggers.Current.MouseRight && !Main.playerInventory)
            {
                // Ensure we have a valid item in RMBSlot
                if (MySlotUI.RMBSlot.Item.type != ItemID.None)
                {
                    // Save the selected item and put RMBSlot.Item in its place, prevent using both items at once
                    if (!isUsingItem && !Player.channel)
                    {
                        originalSelectedItem = Player.inventory[Player.selectedItem];
                        Player.inventory[Player.selectedItem] = MySlotUI.RMBSlot.Item;

                        // Set animation and item time accounting for modifiers and accessories 
                        Player.itemAnimation = Player.itemAnimationMax;
                        Player.itemTime = Player.itemAnimationMax;

                        isUsingItem = true;
                    }

                    // Simulate item use
                    if (Player.itemAnimation <= 0)
                    {
                        Player.controlUseItem = true;
                        Player.controlUseTile = false; // dont ask

                        Item item = Player.HeldItem;
                        if (!Player.HeldItem.channel)
                        {
                            Player.ItemCheck();
                            Main.NewText("Itemcheck!");
                        }
                        else
                        {
                            Player.StartChanneling();
                            Player.TryUpdateChannel(GetPlayerActiveProjectile());
                            Main.NewText("Item Channeled!");
                        }
                    }
                }
            }
            else
            {
                // Stop using RMBSlot.Item and return to originalSelectedItem
                if (isUsingItem)
                {
                    Player.controlUseItem = false;
                    Player.controlUseTile = false;
                    Player.inventory[Player.selectedItem] = originalSelectedItem;
                    isUsingItem = false;
                    Player.TryCancelChannel(GetPlayerActiveProjectile());
                }
            }
        }
        private Projectile GetPlayerActiveProjectile()
        {
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                // Check if the projectile belongs to this player
                if (projectile.owner == Player.whoAmI)
                {
                    Main.NewText("Sent",255,0,0);
                    return projectile;
                }
            }
            return null;
        }
        private void ProjectileExists()
        {
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                // Check if the projectile belongs to this player
                if (projectile.owner == Player.whoAmI)
                {
                    Main.NewText("Found Projectile",255,0,0);
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
