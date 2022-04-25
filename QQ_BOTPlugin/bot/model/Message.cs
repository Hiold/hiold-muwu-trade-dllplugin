using QQ_BOTPlugin.bot.model.message;
using SimpleJson2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model
{
    public class Message
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<object> data { get; set; }

        public Message(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return;
            }
            Message resp = SimpleJson2.SimpleJson2.DeserializeObject<Message>(param);
            this.code = resp.code;
            this.msg = resp.msg;
            this.data = resp.data;

            //开始遍历处理data中每个元素
            if (resp.data != null && resp.data.Count > 0)
            {
                for (var i = 0; i < resp.data.Count; i++)
                {
                    JsonObject tempObj = (JsonObject)resp.data[i];
                    if (tempObj.TryGetValue("type", out object type))
                    {
                        //好友消息
                        if (type.Equals("FriendMessage"))
                        {
                            FriendMessage tm = new FriendMessage(tempObj.ToString());
                            resp.data[i] = tm;
                        }

                        //群临时消息
                        if (type.Equals("TempMessage"))
                        {
                            TempMessage tm = new TempMessage(tempObj.ToString());
                            resp.data[i] = tm;
                        }

                        //群临消息
                        if (type.Equals("GroupMessage"))
                        {
                            GroupMessage tm = new GroupMessage(tempObj.ToString());
                            resp.data[i] = tm;
                        }

                        //其他客户端消息
                        if (type.Equals("TempMessage"))
                        {
                            OtherClientMessage tm = new OtherClientMessage(tempObj.ToString());
                            resp.data[i] = tm;
                        }

                        //陌生人消息
                        if (type.Equals("StrangerMessage"))
                        {
                            StrangerMessage tm = new StrangerMessage(tempObj.ToString());
                            resp.data[i] = tm;
                        }
                    }
                }
            }
        }
        public Message()
        {

        }
    }


}
