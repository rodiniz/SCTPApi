using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Models
{

    public class RootObject
    {
        public string code { get; set; }
        public string name { get; set; }
        public string zone { get; set; }
        public List<Line> lines { get; set; }
        public string geomdesc { get; set; }
        public int mode { get; set; }
        public string address { get; set; }
    }
    public class Line
    {
        public int accessibility { get; set; }
        public string code { get; set; }
        public string pubcode { get; set; }
        public int dir { get; set; }
        public string description { get; set; }
    }

}
