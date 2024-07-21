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
    public CustomItemSlot MyNormalSlot;
    public CustomItemSlot MyAccessorySlot;

    public bool Visible {
        get => Main.playerInventory; // how do you display your slot?
    }

    public override void OnInitialize() {
        // Add a texture to display when the accessory slot is empty
        CroppedTexture2D emptyTexture = new CroppedTexture2D(
            ModContent.Request<Texture2D>("OffHandidiotmodSlotted/MyTexture").Value,
            CustomItemSlot.DefaultColors.EmptyTexture);

        MyNormalSlot = new CustomItemSlot(); // leave blank for a plain inventory space
        MyAccessorySlot = new CustomItemSlot(ItemSlot.Context.EquipAccessory, 0.85f) {
            IsValidItem = item => item.type > ItemID.None, // what do you want in the slot?
            EmptyTexture = emptyTexture,
            HoverText = "RMB Slot" // try to describe what will go into the slot
        };

        // You can set these once or change them in DrawSelf()
        MyNormalSlot.Left.Set(100, 0);
        MyNormalSlot.Top.Set(100, 0);

        MyAccessorySlot.Left.Set(150, 0);
        MyAccessorySlot.Top.Set(100, 0);

        // Don't forget to add them to the UIState!
        Append(MyNormalSlot);
        Append(MyAccessorySlot);

        // If you're going to hook into CustomItemSlot events, put them here, then unload them during MyMod.Unload()
        MyNormalSlot.ItemChanged += ItemChanged;
    }

    private void ItemChanged(CustomItemSlot slot, ItemChangedEventArgs e) {
        // It's usually best to "encapsulate" data: that is, let the class that owns it handle it, while calling only
        // public functions
        Main.LocalPlayer.GetModPlayer<MyCustomSlotPlayer>().ItemChanged(slot, e);
    }

    // Unload the class by removing its event handlers
    internal void Unload() {
        MyNormalSlot.ItemChanged -= ItemChanged;
    }
}
}