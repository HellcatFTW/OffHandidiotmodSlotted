using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ID;
using CustomSlot.UI;
using CustomSlot;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

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
        private bool requestExists { get => manualSwapRequested || swapRequestedToMain || swapRequestedToOffhand; }
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
            if (Activation.SwapKeybind.JustPressed && Player.selectedItem != 58 && !isTorchHeld())
            {
                manualSwapRequested = true;
            }
        }
        public bool isTorchHeld()
        {
            Item item = Player.HeldItem;
            return ItemID.Sets.Torches[item.type] || ItemID.Sets.Glowsticks[item.type];

        }
        public bool isUIActive()
        {
            return Main.ingameOptionsWindow || Main.mapFullscreen || Main.gamePaused;
        }
        public override void PreUpdate()
        {
            bool shiftCurrent = Main.keyState.PressingShift();
            bool actualMouseLeftCurrent = PlayerInput.Triggers.Current.MouseLeft;
            var actualMouseLeftJustPressed = actualMouseLeftCurrent && !previousMouseLeft;
            var actualMouseLeftJustReleased = !actualMouseLeftCurrent && previousMouseLeft;

            // Issues:
            //
            // Remake the entire thing using Mirsario's overhaul's PlayerItemUse.....
            //1- Needs a toggle to choose which attack of a weapon it uses
            //6- having a config that does allow you to use both items at once (1: do we care? thats just stealing yoraizors idea at this point, + idk if we can even do on current implementation)
            //7- going click by click for people who dont have autofire enabled does some stupid shit. maybe thats what that guy was complaining about in issue 5
            //8- (DONE) if hotbar slot is empty, and magic key is pressed, item gets swapped and nothing happens 
            //9- (DONE) player.heldItem must not be torch, otherwise some weird shit happens which ill explain. basically dont swap items if held item is torch or shift is held (ItemID.Sets.Torches[item.type]) 
            //10- (DONE) items swap if inventory is open. Check if inventory is open when magic key is pressed to send a swapRequest.
            //11- (DONE) setting use off hand item to mouse1(lmb) prevents you from using GUI mouse1 functions. can temporarily try to block mouse1 from being assigned? but the real fix is to use mirsario's implementation 
            //
            //12- (DONE)swapping to prism via magic key then releasing, still uses mouse after swap
            //13- if you mine blocks they dont get stacked back into offhandslot
            //14- change slot color
            //15- make slot draggable(?) or move it
            //16- maybe account for resolution and UI Scale?
            //17- disable mod if Main.netMode > 0
            //
            //================================================================================================================================================


            
            // Offhand function: This simulates LMB. Prevents vanilla interference and duplication by disallowing if inventory is open or mouse has an item in it
            // also disables mouse simulating if UI is open to prevent locking player in their settings menu
            if (Activation.UseOffhandKeybind.Current && !isUIActive() && !requestExists && !Main.playerInventory)
            {
                // Ensure we have a valid item in RMBSlot and nor torch nor mouseitem is held, as well as being in main hand state
                if (currentlySwapped)
                {
                    // Swap and use
                    if (!currentlySwapped && Player.selectedItem != 58 && !isTorchHeld())
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

            if (manualSwapRequested)  // for Swap Slots keybind 'T', don't touch it.  DONT TOUCH DONT TOUCH DONT TOUCH DONT TOUCH
            {
                bool cancelCurrent = false;
                if (TrySwap(out cancelCurrent))
                {
                    manualSwapRequested = false;
                }
                else if (cancelCurrent)
                {
                    manualSwapRequested = false;
                }
            }    // DONT TOUCH DONT TOUCH DONT TOUCH DONT TOUCH DONT TOUCH DONT TOUCH DONT TOUCH DONT TOUCH DONT TOUCH DONT TOUCH


            // Input handlers:
            // we may not swap to empty off-hand
            // (swapping back to empty main hand is fine)
            // if both are held, the most recently pressed takes precedence  


            // General input handler, assuming mod should be active and no pause / menus open that require cursor clicks
            if (!shiftCurrent && !isUIActive() && !Main.playerInventory)
            {
                // Handles magic key state 
                if (!currentlySwapped && Activation.UseOffhandKeybind.JustPressed && MySlotUI.RMBSlot.Item.type != ItemID.None) // 1: No offhand, keybind just pressed, switches from main to off 
                {
                    swapRequestedToOffhand = true;
                }
                if (swapRequestedToOffhand && Activation.UseOffhandKeybind.JustReleased) // reset swapRequestedToOffhand if key is released
                {
                    swapRequestedToOffhand = false;
                }
                if (currentlySwapped && Activation.UseOffhandKeybind.JustReleased && actualMouseLeftCurrent)
                {
                    swapRequestedToMain = true;
                }
                if (actualMouseLeftJustReleased && Activation.UseOffhandKeybind.Current && !currentlySwapped && MySlotUI.RMBSlot.Item.type != ItemID.None)
                {
                    swapRequestedToOffhand = true;
                }




                // Handles left mouse state 
                if (currentlySwapped && actualMouseLeftJustPressed) //2: Offhand active and keybind released, Switches from off to main
                {
                    swapRequestedToMain = true;
                }
                if (swapRequestedToMain && actualMouseLeftJustReleased) // reset swapRequestedToMain if key is released
                {
                    swapRequestedToMain = false;
                }
                if (!actualMouseLeftCurrent && !Activation.UseOffhandKeybind.Current && currentlySwapped)
                {
                    swapRequestedToMain = true;
                }
                if (actualMouseLeftJustPressed && Activation.UseOffhandKeybind.Current && currentlySwapped)
                {
                    swapRequestedToMain = true;
                }
            }


            // Swap request handler
            if (swapRequestedToMain || swapRequestedToOffhand)
            {
                bool cancelCurrent = false;
                //PrintStates();
                if (TrySwap(out cancelCurrent))
                {
                    swapRequestedToOffhand = false;
                    swapRequestedToMain = false;
                    currentlySwapped = !currentlySwapped;
                    //PrintStates();
                }
                else if (cancelCurrent)
                {
                    swapRequestedToOffhand = false;
                    swapRequestedToMain = false;
                }
            }


            // MOUSE DEBUGGING STUFFFF
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



            previousMouseLeft = actualMouseLeftCurrent; // save mouse state for next tick lol
        }  //end of preupdate



        public void PrintStates()
        {
            Main.NewText("offhand: {" + currentlySwapped.ToString() + "}, swapRequestedToOffhand: {" + swapRequestedToOffhand.ToString() + "},"
            + "swapRequestedToMain: {" + swapRequestedToMain.ToString() + "}");
        }






        // swaps at earliest possible moment while looking.. ok. prevent funny torch business
        public bool TrySwap(out bool cancelSwapRequests)
        {
            if (!isTorchHeld() && !isUIActive() && Player.selectedItem != 58)
            {
                if (Player.HeldItem.channel)
                {
                    // We block input here to allow channeled items such as last prism to wind down from their positive animation time back to 0...
                    PlayerInput.Triggers.JustReleased.MouseLeft = false;
                    PlayerInput.Triggers.JustPressed.MouseLeft = false;
                    PlayerInput.Triggers.Current.MouseLeft = false;
                }
                if (!Player.channel)  // current item is not channeling
                {
                    if (Player.ItemAnimationEndingOrEnded)
                    {
                        // Here, we block inputs again to make sure there is an opportunity to swap the items smoothly (one frame with no item being used) 
                        // without doing weird animation things.
                        PlayerInput.Triggers.JustReleased.MouseLeft = false;
                        PlayerInput.Triggers.JustPressed.MouseLeft = false;
                        PlayerInput.Triggers.Current.MouseLeft = false;
                        SwapSlots();
                        cancelSwapRequests = false;
                        return true;
                    }
                }
                else if (Player.channel) // current item is channeling, sends false to keep mouse buttons up there set to false.
                {
                    cancelSwapRequests = false;
                    return false;
                }
                cancelSwapRequests = false;
                return false;
            }
            else
            {
                cancelSwapRequests = true;
                return false;
            }

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
