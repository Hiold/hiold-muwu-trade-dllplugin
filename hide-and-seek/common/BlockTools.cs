using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hide_and_seek.common
{
    public class BlockTools
    {
        /// <summary>
        /// 放置方块
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="blockName"></param>
        public static void SetBlock(Entity entity, Vector3i pos, string blockName)
        {
            //上移一格
            //pos.y++;
            BlockValue bv = Block.GetBlockValue(blockName);
            Vector3i finalPos = searchLocation(new Vector3i(entity.position));
            GameManager.Instance.World.SetBlockRPC(finalPos, bv);
            MainController.HidersPos[entity.entityId] = finalPos;
        }

        /// <summary>
        /// 移除方块
        /// </summary>
        /// <param name="pos"></param>
        public static void RemoveBlock(int entityid, Vector3i pos)
        {
            //上移一格
            //pos.y++;
            BlockValue bv = Block.GetBlockValue("air");
            GameManager.Instance.World.SetBlockRPC(pos, bv);
            MainController.HidersPos.Remove(entityid);
        }


        public static Vector3i searchLocation(Vector3i pos)
        {
            Vector3i front = new Vector3i(pos.x + 1, pos.y + 1, pos.z);
            Vector3i behand = new Vector3i(pos.x - 1, pos.y + 1, pos.z);
            Vector3i left = new Vector3i(pos.x, pos.y + 1, pos.z + 1);
            Vector3i right = new Vector3i(pos.x, pos.y + 1, pos.z - 1);
            BlockValue bvfront = GameManager.Instance.World.GetBlock(front);
            BlockValue bvbehand = GameManager.Instance.World.GetBlock(behand);
            BlockValue bvleft = GameManager.Instance.World.GetBlock(left);
            BlockValue bvright = GameManager.Instance.World.GetBlock(right);
            if (bvfront.isair)
            {
                Vector3i tmpgroundPos = new Vector3i(front.x, front.y - 1, front.z);
                BlockValue ground = GameManager.Instance.World.GetBlock(tmpgroundPos);
                if (!ground.isair && !ground.isWater)
                {
                    return front;
                }
            }

            if (bvbehand.isair)
            {
                Vector3i tmpgroundPos = new Vector3i(behand.x, behand.y - 1, behand.z);
                BlockValue ground = GameManager.Instance.World.GetBlock(tmpgroundPos);
                if (!ground.isair && !ground.isWater)
                {
                    return behand;
                }
            }

            if (bvleft.isair)
            {
                Vector3i tmpgroundPos = new Vector3i(left.x, left.y - 1, left.z);
                BlockValue ground = GameManager.Instance.World.GetBlock(tmpgroundPos);
                if (!ground.isair && !ground.isWater)
                {
                    return left;
                }
            }

            if (bvright.isair)
            {
                Vector3i tmpgroundPos = new Vector3i(right.x, right.y - 1, right.z);
                BlockValue ground = GameManager.Instance.World.GetBlock(tmpgroundPos);
                if (!ground.isair && !ground.isWater)
                {
                    return right;
                }
            }

            return new Vector3i(0, 0, 0);
        }
    }
}
