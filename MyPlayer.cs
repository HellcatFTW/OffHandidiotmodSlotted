using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.ModLoader.IO;

namespace OffHandidiotmodSlotted
{
    public class MyPlayer : ModPlayer
    {
        public Item customSlotItem = new Item(); // Ensure this is initialized

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("CustomSlotItem", out TagCompound itemTag))
            {
                customSlotItem = ItemIO.Load(itemTag);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            TagCompound itemTag = ItemIO.Save(customSlotItem);
            tag["CustomSlotItem"] = itemTag;
        }

        public override void PostUpdate()
        {
            // Custom logic here if needed
        }
    }
}
