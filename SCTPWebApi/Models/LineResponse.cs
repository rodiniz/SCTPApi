using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Models
{
    public class Record
    {
        public int accessibility { get; set; }
        public string code { get; set; }
        public string pubcode { get; set; }
        public string description { get; set; }

        public string zone { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public int sequence { get; set; }


    }

    public class LineResponse
    {
        public object sort { get; set; }
        public int recordsReturned { get; set; }
        public int totalRecords { get; set; }
        public List<Record> records { get; set; }

        public int startIndex { get; set; }
        public string dir { get; set; }
    }

}
