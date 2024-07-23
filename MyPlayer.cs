using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ID;
using CustomSlot.UI;
using CustomSlot;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Mono.Cecil.Cil;
using System.Security.Policy;
using Terraria.WorldBuilding;
using System;

namespace OffHandidiotmod
{
    public class MyPlayer : ModPlayer
    {
        private int delayTimerOffhand = 0;
        private int delayTimerMessage = 0;
        private bool currentlySwapped = false;
        private bool swapRequestedToOffhand = false;
        private bool swapRequestedToMain = false;
        private bool manualSwapRequested = false;
        private bool previousMouseLeft;

        public bool IsMessageEnabled()
        {
            return ModContent.GetInstance<OffHandConfig>().ChatMessageToggle;
        }
        public override void OnEnterWorld()
        {
            if (IsMessageEnabled())
            {
                delayTimerMessage = 160;
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Activation.SwapKeybind.JustPressed && Player.selectedItem != 58)
            {
                manualSwapRequested = true;
            }
        }
        public override void PreUpdate()
        {
            bool actualMouseLeftCurrent = PlayerInput.Triggers.Current.MouseLeft;
            var actualMouseLeftJustPressed = actualMouseLeftCurrent && !previousMouseLeft;
            var actualMouseLeftJustReleased = !actualMouseLeftCurrent && previousMouseLeft;
            // Issue:
            // Holding LMB then pressing the magic key causes both items to alternate in swinging, but with perfect timing
            // Holding magic key then pressing LMB ignores the LMB press, as if it was from Offhand function
            // Need to make TryInterrupt() func to fix this, where it continously sets Current.MouseLeft to false until it recieves that swap succeeded
            // 
            // if there's an active projectile from a sniper, while Player.itemanimationactive, and user presses magic key, game will swap the items permanently and jitter view.
            //
            // if holding LMB and then hold magic key and release LMB, nothing happens, but it should stop using LMB and start using magic key one.
            //
            // && !Player.ItemAnimationActive in swap request number 1 is to prevent item swapping during LMB anim from a single magic key press (must fix)
            //
            //
            //
            // Make going from offhand to main slow like main to offhand or make both quick.
            //
            //
            //

            if (manualSwapRequested)  // for Swap Slots keybind 'T', don't touch it.
            {
                if (TrySwap())
                {
                    manualSwapRequested = false;
                }
            }


            // Handles magic key state
            if (!currentlySwapped && Activation.UseOffhandKeybind.JustPressed && MySlotUI.RMBSlot.Item.type != ItemID.None) // 1: No offhand, keybind just pressed, switches from main to off 
            {
                swapRequestedToOffhand = true;
            }
            if (swapRequestedToOffhand && Activation.UseOffhandKeybind.JustReleased) // reset swapRequestedToOffhand if key is released
            {
                swapRequestedToOffhand = false;
            }













            // Handles left mouse state 
          // if (currentlySwapped && actualMouseLeftJustPressed && MySlotUI.RMBSlot.Item.type != ItemID.None) //2: Offhand active and keybind released, Switches from off to main
          // {
          //     swapRequestedToMain = true;
          // }
          // if (swapRequestedToMain && actualMouseLeftJustReleased) // reset swapRequestedToMain if key is released
          // {
          //     swapRequestedToMain = false;
          // }
            if (!actualMouseLeftCurrent && !Activation.UseOffhandKeybind.Current && currentlySwapped)
            {
                swapRequestedToMain = true;
            }
          // if (actualMouseLeftJustPressed && Activation.UseOffhandKeybind.Current && currentlySwapped)
          // {
          //     swapRequestedToMain = true;
          // }


            // Swap request handler
            if (swapRequestedToMain || swapRequestedToOffhand)
            {
                //PrintStates();
                if (TrySwap())
                {
                    swapRequestedToOffhand = false;
                    swapRequestedToMain = false;
                    currentlySwapped = !currentlySwapped;
                    //PrintStates();
                }
            }





            //================================================================================================================================================


            // Offhand function: This simulates LMB. Prevents vanilla interference and duplication by disallowing if inventory is open or mouse has an item in it
            if (Activation.UseOffhandKeybind.Current && !Main.playerInventory)
            {
                // Ensure we have a valid item in RMBSlot
                if (MySlotUI.RMBSlot.Item.type != ItemID.None)
                {
                    // Swap and use
                    if (!currentlySwapped && Player.selectedItem != 58)
                    {
                        delayTimerOffhand = 1; // 1-tick delay to allow autopause and whatever else to interrupt
                    }
                    //----------------------------
                    if (delayTimerOffhand >= 0)
                    {
                        delayTimerOffhand--;
                    }
                    else
                    {
                        PlayerInput.Triggers.Current.MouseLeft = true;
                    }

                }
            }

            //================================================================================================================================================




            // if (PlayerInput.Triggers.Current.MouseLeft)
            // {
            //     Main.NewText("Fake Mouse Current:" + PlayerInput.Triggers.Current.MouseLeft.ToString());
            // }
            // if (PlayerInput.Triggers.JustPressed.MouseLeft)
            // {
            //     Main.NewText("Fake Mouse Press:" + PlayerInput.Triggers.JustPressed.MouseLeft.ToString());
            // }
            // if (PlayerInput.Triggers.JustReleased.MouseLeft)
            // {
            //     Main.NewText("Fake Mouse Release:" + PlayerInput.Triggers.JustReleased.MouseLeft.ToString());
            // }
            //if (actualMouseLeftCurrent)
            //{
            //    Main.NewText("Actual Mouse Current:" + actualMouseLeftCurrent.ToString());
            //}




            //Message timer
            if (delayTimerMessage > 0) // Warning message delay
            {
                delayTimerMessage--;
            }
            if (delayTimerMessage == 1 && IsMessageEnabled()) // Warning message send
            {
                Main.NewText("Please make sure you've set Offhand Slot's keybinds in your controls. You can disable this message in Mod Configuration.", 255, 255, 0);
            }



            previousMouseLeft = actualMouseLeftCurrent;
        }

        public void PrintStates()
        {
            Main.NewText("offhand: {" + currentlySwapped.ToString() + "}, swapRequestedToOffhand: {" + swapRequestedToOffhand.ToString() + "},"
            + "swapRequestedToMain: {" + swapRequestedToMain.ToString() + "}");
        }


        // swaps at earliest possible moment while looking.. ok
        public bool TrySwap()
        {
            if ((Player.selectedItem != 58) && !Player.channel)
            {
                if (Player.ItemAnimationEndingOrEnded)             //polish feature, not needed but prevents 2 items from being swung together
                {
                    // PlayerInput.Triggers.Current.MouseLeft = false;  //idk if its needed but it worked before and im keeping it
                    SwapSlots();
                    return true;
                }
            }
            else if ((Player.selectedItem != 58) && Player.channel)
            {
                SwapSlots();
                return true;
            }
            return false;
        }

        public void SwapSlots()
        {
            Item originalSelectedItem = Player.inventory[Player.selectedItem];
            Player.inventory[Player.selectedItem] = MySlotUI.RMBSlot.Item;
            MySlotUI.RMBSlot.SetItem(originalSelectedItem);
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
