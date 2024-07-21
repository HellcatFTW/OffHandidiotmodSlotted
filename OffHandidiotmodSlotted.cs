using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace OffHandidiotmodSlotted
{
    public class OffHandidiotmodSlotted : Mod
    {
        public static CustomSlotUI CustomSlotUI { get; private set; }

        public override void Load()
        {
            if (Main.dedServ) return;

            // Initialize the UI state
            CustomSlotUI = new CustomSlotUI();
            CustomSlotUI.Activate();
        }

        public override void Unload()
        {
            if (Main.dedServ) return;

            CustomSlotUI?.Deactivate();
            CustomSlotUI = null;
        }
    }
}
