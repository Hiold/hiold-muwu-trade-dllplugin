using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model.message
{
    public class Source
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int time { get; set; }
    }

    public class Quote
    {

        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int groupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int senderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int targetId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OriginItem> origin { get; set; }
    }

    public class OriginItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string text { get; set; }

    }

    public class At
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int target { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string display { get; set; }

    }

    public class AtAll
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
    }

    public class Face
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int faceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
    }

    public class Plain
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string text { get; set; }
    }

    public class Image
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string imageId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string base64 { get; set; }
    }

    public class FlashImage
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string imageId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string base64 { get; set; }
    }

    public class Voice
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string voiceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string base64 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int length { get; set; }
    }

    public class Xml
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string xml { get; set; }
    }

    public class Json
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string json { get; set; }
    }

    public class App
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string content { get; set; }
    }

    public class Poke
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
    }

    public class Dice
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int value { get; set; }
    }

    public class MarketFace
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 商城表情
        /// </summary>
        public string name { get; set; }
    }

    public class MusicShare
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string kind { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string jumpUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string pictureUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string musicUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string brief { get; set; }
    }

    public class ForwardMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ForwardMessageNodeListItem> nodeList { get; set; }
    }
    public class ForwardMessageNodeListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int senderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string senderName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> messageChain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int messageId { get; set; }

    }

    public class File
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int size { get; set; }
    }

    public class MiraiCode
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
    }
}
