using SimpleJson2;
using System;
using System.IO;

namespace HioldMod.src.Serialize
{
    class JsonUtils
    {
        public static string SerializeObject(object _item)
        {
            return SimpleJson2.SimpleJson2.SerializeObject(_item);
        }

        /// <summary>
        /// 从ItemStack解析string
        /// </summary>
        /// <param name="itemStack">原始物品信息</param>
        /// <returns>物品string信息</returns>
        public static string ByteStringFromItem(ItemStack[] itemStack)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            GameUtils.WriteItemStack(bw, itemStack);
            var hex = BitConverter.ToString(ms.ToArray(), 0).Replace("-", string.Empty).ToLower();
            return hex;
        }

        /// <summary>
        /// 从string获取ItemStack信息
        /// </summary>
        /// <param name="itemString">原始物品信息</param>
        /// <returns>Unity物品</returns>
        public static ItemStack[] ItemFromString(string itemString)
        {
            MemoryStream ms = new MemoryStream(HexToByte(itemString));
            BinaryReader br = new BinaryReader(ms);
            ItemStack[] resultStack = GameUtils.ReadItemStack(br);
            return resultStack;
        }


        public static byte[] HexToByte(string hexString)
        {
            //運算後的位元組長度:16進位數字字串長/2
            byte[] byteOUT = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i = i + 2)
            {
                //每2位16進位數字轉換為一個10進位整數
                byteOUT[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return byteOUT;
        }

    }
}
