using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Entity.cs 關鍵字物件
namespace _1999.Serialization
{
    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public double score { get; set; }
    }
}