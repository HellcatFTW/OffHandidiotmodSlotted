using CustomSlot;
using CustomSlot.UI;
using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace OffHandidiotmodSlotted
{
public class MySlotUI : UIState {
    public static CustomItemSlot RMBSlot;

    public bool Visible {
        get => Main.playerInventory; // how do you display your slot?
    }

    public override void OnInitialize() {

        RMBSlot = new CustomItemSlot(ItemSlot.Context.InventoryItem,0.85f) {
            IsValidItem = item => item.type > ItemID.None, // what do you want in the slot?
            HoverText = "RMB Slot" // try to describe what will go into the slot
        };; // leave blank for a plain inventory space


        // You can set these once or change them in DrawSelf()
        RMBSlot.Left.Set(20, 0);
        RMBSlot.Top.Set(300, 0);

        // Don't forget to add them to the UIState!
        Append(RMBSlot);


        // If you're going to hook into CustomItemSlot events, put them here, then unload them during MyMod.Unload()
        RMBSlot.ItemChanged += ItemChanged;
    }

    private void ItemChanged(CustomItemSlot slot, ItemChangedEventArgs e) {
        // It's usually best to "encapsulate" data: that is, let the class that owns it handle it, while calling only
        // public functions
        Main.LocalPlayer.GetModPlayer<MyCustomSlotPlayer>().ItemChanged(slot, e);
    }

    // Unload the class by removing its event handlers
    internal void Unload() {
        RMBSlot.ItemChanged -= ItemChanged;
    }
}
}