using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Runtime.Serialization;
using Microsoft.Bot.Connector;//
using _1999.Serialization; //


//Utterance.cs 表示整個Luis傳回Json物件
namespace _1999.Serialization
{
    [DataContract]
    public class Utterance
    {
        [DataMember]
        public string query { get; set; }
        [DataMember]
        public List<Intent> intents { get; set; }
        [DataMember]
        public List<Entity> entities { get; set; }
    }
}