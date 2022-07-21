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
        public static void SetBlock(Vector3i pos, string blockName)
        {
            //上移一格
            //pos.y++;
            BlockValue bv = Block.GetBlockValue(blockName);
            GameManager.Instance.World.SetBlockRPC(pos, bv);
        }

        /// <summary>
        /// 移除方块
        /// </summary>
        /// <param name="pos"></param>
        public static void RemoveBlock(Vector3i pos)
        {
            //上移一格
            //pos.y++;
            BlockValue bv = Block.GetBlockValue("air");
            GameManager.Instance.World.SetBlockRPC(pos, bv);
        }
    }
}
