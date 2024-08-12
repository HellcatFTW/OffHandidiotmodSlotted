using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ID;
using CustomSlot.UI;
using CustomSlot;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using System;
using Terraria.Localization;
using System.Linq;

namespace OffHandidiotmod
{
    public class MyPlayer : ModPlayer
    {
        private int delayTimerMessage = 0;
        private bool currentlySwapped = false;
        private int? originalMainItemSlot;
        private bool swapRequestedToOffhand = false;
        private bool swapRequestedToMain = false;
        private bool manualSwapRequested = false;
        private bool requestExists { get => manualSwapRequested || swapRequestedToMain || swapRequestedToOffhand; }
        private bool previousMouseLeft;
        private bool interactableIconShown;
        public static bool blacklistChanged = false;
        private bool[] blacklistSet = [];
        private bool blockRMB = false;

        public bool IsMessageEnabled() // message enabled in config or not, all instances where this is called should have a remote player exit
        {
            return OffHandConfig.Instance.ChatMessageToggle;
        }
        public override void OnEnterWorld() // starts message timer to be after some other mods if message is enabled and player is not remote
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }
            if (IsMessageEnabled())
            {
                delayTimerMessage = 160;
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet) // called by game automatically, checks for manual swap keybind and acts accordingly if not remote.
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }
            if (Activation.SwapKeybind.JustPressed && Main.LocalPlayer.selectedItem != 58 && !IsTorchHeld())
            {
                manualSwapRequested = true;
            }
        }
        public bool IsTorchHeld() // torch hold check
        {
            if (Player.whoAmI != Main.myPlayer) // throws exception if player is remote somehow at time of check.
            {
                string ContactDev = Language.GetTextValue("Mods.OffHandidiotmod.TextMessages.ContactDev");
                throw new Exception($"Code 762: {ContactDev}");
            }
            Item item = Main.LocalPlayer.HeldItem;
            return ItemID.Sets.Torches[item.type] || ItemID.Sets.Glowsticks[item.type];

        }
        public bool IsUIActive()
        {
            if (Player.whoAmI != Main.myPlayer) // throws exception if player is remote somehow at time of check.
            {
                string ContactDev = Language.GetTextValue("Mods.OffHandidiotmod.TextMessages.ContactDev");
                throw new Exception($"Code 556: {ContactDev}");
            }
            return Main.ingameOptionsWindow || Main.mapFullscreen || Main.gamePaused || Main.LocalPlayer.mouseInterface;
        }
        public bool IsPlayerAboutToInteract() //checks if mouse is on a door, switch, or container etc
        {
            if (isKeybindMouseLeftOrRight())
            {
                Microsoft.Xna.Framework.Point tileCoords = Main.MouseWorld.ToTileCoordinates();
                Tile tile = Main.tile[tileCoords.X, tileCoords.Y];
                if (!Main.SmartCursorShowing)
                {
                    if (interactableIconShown)
                    {
                        return true;
                    }
                }
                if (Main.HoveringOverAnNPC)
                {
                    return true;
                }
                if (Main.LocalPlayer.mouseInterface)
                {
                    return true;
                }
                if (Main.SmartInteractTileCoords.Count != 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool isKeybindMouseLeftOrRight()
        {
            var offhandkeybindlist = Activation.UseOffhandKeybind.GetAssignedKeys();
            return offhandkeybindlist.Contains("Mouse2") || offhandkeybindlist.Contains("Mouse1");
        }
        public void UpdateBlacklist()
        {
            if (blacklistChanged)
            {
                var itemIDs = OffHandConfig.Instance.ItemBlacklist.Where(val => !val.IsUnloaded).Select(val => val.Type).ToArray();
                blacklistSet = ItemID.Sets.Factory.CreateBoolSet(itemIDs);
                blacklistChanged = false;
            }
        }

        public override void PreUpdate()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }
            bool shiftCurrent = Main.keyState.PressingShift();
            bool actualMouseLeftCurrent = PlayerInput.Triggers.Current.MouseLeft;
            var actualMouseLeftJustPressed = actualMouseLeftCurrent && !previousMouseLeft;
            var actualMouseLeftJustReleased = !actualMouseLeftCurrent && previousMouseLeft;

            // Issues:
            //
            // Remake the entire thing using Mirsario's overhaul's PlayerItemUse.....
            //  1- Needs a toggle to choose which attack of a weapon it uses
            //  6- having a config that does allow you to use both items at once (1: do we care? thats just becoming yoraizors at this point, + idk if we can even do on current implementation)
            //  7- (DONE)going click by click for people who dont have autofire enabled does some stupid shit. maybe thats what that guy was complaining about in issue 5
            //  8- (DONE) if hotbar slot is empty, and magic key is pressed, item gets swapped and nothing happens 
            //  9- (DONE) player.heldItem must not be torch, otherwise some weird shit happens which ill explain. basically dont swap items if held item is torch or shift is held (ItemID.Sets.Torches[item.type]) 
            // 10- (DONE) items swap if inventory is open. Check if inventory is open when magic key is pressed to send a swapRequest.
            // 11- (DONE) setting use off hand item to mouse1(lmb) prevents you from using GUI mouse1 functions. can temporarily try to block mouse1 from being assigned? but the real fix is to use mirsario's implementation 
            // 12- (DONE)swapping to prism via magic key then releasing, still uses mouse after swap
            // 19- (DONE) When containers are opened via RMB and 'Use Offhand Item' is also bound to RMB, your selected hotbar slot and offhand are briefly swapped.
            // 17- (DONE)if you click magic key quickly then hold after releasing said fast click, item will shoot once and not continue.
            // 20- (DONE)grabbing item from slot if inventory is not open causes weird behaviour because terraria disallows holding items if inventory is closed.
            // 21- (DONE)check if wall is held in mouseitem
            //14- change slot color or texture

            // BIG problem to tackle:
            //-19 items dont stack into offhand slot
            //            Solution: link offhand slot item from inventory instead of actually having it there, make it very clear it isnt a real item.
            //
            //================================================================================================================================================


            UpdateBlacklist();

            if (blacklistSet[Player.HeldItem.type])
            {
                if (isKeybindMouseLeftOrRight())
                {
                    blockRMB = true;
                }
                else
                {
                    blockRMB = false;
                }
            }
            else
            {
                blockRMB = false;
            }

            // Offhand function: This simulates LMB. Prevents vanilla interference and duplication by disallowing if inventory is open or mouse has an item in it
            // also disables mouse simulating if UI is open to prevent locking player in their settings menu
            if (Activation.UseOffhandKeybind.Current && !IsUIActive() && !requestExists && !IsPlayerAboutToInteract())
            {
                if (currentlySwapped)
                {
                    PlayerInput.Triggers.Current.MouseLeft = true;
                }
            }
            //================================================================================================================================================

            if (manualSwapRequested)  // for Swap Slots keybind 'Q', don't touch it.  DONT TOUCH DONT TOUCH DONT TOUCH DONT TOUCH
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
            if (!shiftCurrent && !IsUIActive() && !IsPlayerAboutToInteract())
            {
                // Handles magic key state 
                if (!currentlySwapped && Activation.UseOffhandKeybind.JustPressed && MySlotUI.RMBSlot.Item.type != ItemID.None && !blockRMB) // 1: No offhand, keybind just pressed, switches from main to off 
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
                if (actualMouseLeftJustReleased && Activation.UseOffhandKeybind.Current && !currentlySwapped && MySlotUI.RMBSlot.Item.type != ItemID.None && !blockRMB)
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
                if (swapRequestedToMain && !actualMouseLeftCurrent && Activation.UseOffhandKeybind.Current && currentlySwapped)
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
                if (TrySwap(out cancelCurrent))
                {
                    swapRequestedToOffhand = false;
                    swapRequestedToMain = false;
                    currentlySwapped = !currentlySwapped;
                }
                else if (cancelCurrent)
                {
                    swapRequestedToOffhand = false;
                    swapRequestedToMain = false;
                }
            }

            //Message timer
            if (delayTimerMessage > 0) // Warning message delay
            {
                delayTimerMessage--;
            }
            if (delayTimerMessage == 1 && IsMessageEnabled()) // Warning message send
            {
                string SpawnMessage = Language.GetTextValue("Mods.OffHandidiotmod.TextMessages.SpawnMessage");
                Main.NewText(SpawnMessage, 255, 255, 0);
            }



            previousMouseLeft = actualMouseLeftCurrent; // save mouse state for next tick lol
        }  //end of preupdate
        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }

            if (Player.HeldItem.pick == 0 && Player.HeldItem.axe == 0 && Player.HeldItem.hammer == 0 && Player.HeldItem.createTile == -1
            && Player.HeldItem.createWall == -1 && Player.HeldItem.type != ItemID.Clentaminator && Player.HeldItem.type != ItemID.Clentaminator2)
            {
                interactableIconShown = Player.cursorItemIconEnabled;
            }
            else
            {
                interactableIconShown = false;
            }
        }




        // swaps at earliest possible moment while looking.. ok. prevent funny torch business
        // only called when local.
        public bool TrySwap(out bool cancelSwapRequests)
        {
            if (!IsTorchHeld() && !IsUIActive() && Player.selectedItem != 58)
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
            if (Player.whoAmI != Main.myPlayer)
            {
                string ContactDev = Language.GetTextValue("Mods.OffHandidiotmod.TextMessages.ContactDev");
                throw new Exception($"Code 939: {ContactDev}");
            }
            if (swapRequestedToOffhand)
            {
                if (originalMainItemSlot != null) // this should also literally never happen so it prints an error code to chat
                {
                    string ContactDev = Language.GetTextValue("Mods.OffHandidiotmod.TextMessages.ContactDev");
                    Main.NewText($"Code 338: {ContactDev}");
                }
                originalMainItemSlot = Player.selectedItem;
                IndiscriminateSwap();
            }


            if (swapRequestedToMain) // Swap requested to main: Must check if current slot has the correct offhand item, if not, swap the correct ones not the new ones
            {
                if (originalMainItemSlot == null) // this should literally never happen so it prints an error code to chat
                {
                    IndiscriminateSwap();

                    string ContactDev = Language.GetTextValue("Mods.OffHandidiotmod.TextMessages.ContactDev");
                    Main.NewText($"Code 918: {ContactDev}");
                }
                else
                {
                    Item originalSelectedItem = Player.inventory[(int)originalMainItemSlot];
                    Player.inventory[(int)originalMainItemSlot] = MySlotUI.RMBSlot.Item;
                    MySlotUI.RMBSlot.SetItem(originalSelectedItem);
                    originalMainItemSlot = null;
                }
            }
            if (manualSwapRequested)
            {
                IndiscriminateSwap();
            }
        }

        public void IndiscriminateSwap()
        {
            if (Player.whoAmI != Main.myPlayer)
            {
                string ContactDev = Language.GetTextValue("Mods.OffHandidiotmod.TextMessages.ContactDev");
                throw new Exception($"Code 939: {ContactDev}");
            }
            Item originalSelectedItem = Player.inventory[Player.selectedItem];
            Player.inventory[Player.selectedItem] = MySlotUI.RMBSlot.Item;
            MySlotUI.RMBSlot.SetItem(originalSelectedItem);
        }


    }// End of player class









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
            if (Player.whoAmI != Main.myPlayer)
            {
                return;
            }
            MySlotUI.RMBSlot.SetItem(new Item());
        }

        public void ItemChanged(CustomItemSlot slot, ItemChangedEventArgs e)
        {
            // Here we update myCustomItem when RMBSlot fires ItemChanged
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
