using QQ_BOTPlugin.bot.model.message;
using SimpleJson2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model
{
    public class MessageParser
    {
        public static List<object> ParseMessage(List<object> param)
        {
            List<object> result = new List<object>();
            //开始遍历处理data中每个元素
            if (param != null && param.Count > 0)
            {
                for (var i = 0; i < param.Count; i++)
                {
                    JsonObject tempObj = (JsonObject)param[i];
                    if (tempObj.TryGetValue("type", out object type))
                    {
                        if (type.Equals("Source"))
                        {
                            Source tm = SimpleJson2.SimpleJson2.DeserializeObject<Source>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("Quote"))
                        {
                            Quote tm = SimpleJson2.SimpleJson2.DeserializeObject<Quote>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("At"))
                        {
                            At tm = SimpleJson2.SimpleJson2.DeserializeObject<At>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if(type.Equals("AtAll"))
                        {
                            AtAll tm = SimpleJson2.SimpleJson2.DeserializeObject<AtAll>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("Face"))
                        {
                            Face tm = SimpleJson2.SimpleJson2.DeserializeObject<Face>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("Plain"))
                        {
                            Plain tm = SimpleJson2.SimpleJson2.DeserializeObject<Plain>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("Image"))
                        {
                            Image tm = SimpleJson2.SimpleJson2.DeserializeObject<Image>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("FlashImage"))
                        {
                            FlashImage tm = SimpleJson2.SimpleJson2.DeserializeObject<FlashImage>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("Voice"))
                        {
                            Voice tm = SimpleJson2.SimpleJson2.DeserializeObject<Voice>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("Xml"))
                        {
                            Xml tm = SimpleJson2.SimpleJson2.DeserializeObject<Xml>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("Json"))
                        {
                            Json tm = SimpleJson2.SimpleJson2.DeserializeObject<Json>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("App"))
                        {
                            App tm = SimpleJson2.SimpleJson2.DeserializeObject<App>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("Poke"))
                        {
                            Poke tm = SimpleJson2.SimpleJson2.DeserializeObject<Poke>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("Dice"))
                        {
                            Dice tm = SimpleJson2.SimpleJson2.DeserializeObject<Dice>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("MarketFace"))
                        {
                            MarketFace tm = SimpleJson2.SimpleJson2.DeserializeObject<MarketFace>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("MusicShare"))
                        {
                            MusicShare tm = SimpleJson2.SimpleJson2.DeserializeObject<MusicShare>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("ForwardMessage"))
                        {
                            ForwardMessage tm = SimpleJson2.SimpleJson2.DeserializeObject<ForwardMessage>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("File"))
                        {
                            File tm = SimpleJson2.SimpleJson2.DeserializeObject<File>(tempObj.ToString());
                            result.Add(tm);
                        }

                        if (type.Equals("MiraiCode"))
                        {
                            MiraiCode tm = SimpleJson2.SimpleJson2.DeserializeObject<MiraiCode>(tempObj.ToString());
                            result.Add(tm);
                        }

                    }
                }
            }
            return result;
        }
    }
}
