using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace OffHandidiotmodSlotted
{
    public class CustomSlotHandler : ModPlayer
    {
        public int customSlotItem = ItemID.None; // Store item ID for custom slot

        public override void SaveData(TagCompound tag)
        {
            tag["customSlotItem"] = customSlotItem;
        }

        public override void LoadData(TagCompound tag)
        {
            customSlotItem = tag.GetInt("customSlotItem");
        }
    }
}
