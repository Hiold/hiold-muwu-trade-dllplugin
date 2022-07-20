using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hide_and_seek.common
{
    public class MainController
    {
        //寻找者
        public static List<int> Seekers = new List<int>();
        //躲藏者
        public static List<int> Hiders = new List<int>();

        //躲藏者位置
        public static Dictionary<int, Vector3i> HidersPos = new Dictionary<int, Vector3i>();

        System.Threading.Timer updateDLLtimer = new System.Threading.Timer(new TimerCallback(updateBlock), null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));


        public static void updateBlock(object j)
        {
            foreach (int pid in Hiders)
            {
                Log.Out("当前执行者ID为:" + pid);
                if (MainController.Hiders.Contains(pid))
                {
                    Log.Out("进入SetPosAndQRotFromNetwork_fix");
                    Vector3i newPos = new Vector3i(GameManager.Instance.World.GetEntity(pid).position);
                    Vector3i oldPos = MainController.HidersPos[pid];
                    //如果位置发生变化，更新目标位置 burntWoodRoof
                    if (newPos != oldPos)
                    {
                        BlockTools.RemoveBlock(oldPos);
                        BlockTools.SetBlock(newPos, "burntWoodRoof");
                        MainController.HidersPos[pid] = newPos;
                    }

                }
            }
        }

    }
}
