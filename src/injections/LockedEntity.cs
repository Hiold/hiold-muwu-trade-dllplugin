using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.Reflection
{
    class LockedEntity
    {
        public static Dictionary<TileEntity, int> reflection_lockedTileEntities = null;

        public static bool doReflection()
        {
            FieldInfo field = GameManager.Instance.GetType().GetField("lockedTileEntities", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                reflection_lockedTileEntities = field.GetValue(GameManager.Instance) as Dictionary<TileEntity, int>;
                if (reflection_lockedTileEntities != null)
                {
                    return true;
                }
            }
            return false;
        }


        public static bool ValidateLoot(TileEntitySecureLootContainer loot)
        {
            foreach (TileEntity entity in reflection_lockedTileEntities.Keys)
            {
                Vector3i locked = entity.ToWorldPos();
                Vector3i request = loot.ToWorldPos();
                if (locked.x==request.x&&locked.y==request.y&&locked.z==request.z)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ValidateLoot(TileEntityLootContainer loot)
        {
            foreach (TileEntity entity in reflection_lockedTileEntities.Keys)
            {
                Vector3i locked = entity.ToWorldPos();
                Vector3i request = loot.ToWorldPos();
                if (locked.x == request.x && locked.y == request.y && locked.z == request.z)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
