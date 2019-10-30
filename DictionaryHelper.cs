using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace vocab_tester
{
    public class DictionaryXML_info
    {
        [XmlRoot("info")]
        public class Info
        {
            [XmlElement("version")]
            public Version version { get; set; }

            [XmlElement("file")]
            public File file { get; set; }
        }

        public class Version
        {
            //[XmlText]
            //public int Value { get; set; }

            [XmlAttribute]
            public string number { get; set; }            
        }

        public class File
        {
            //[XmlText]
            //public int Value { get; set; }

            [XmlAttribute]
            public string id { get; set; }
        }
    }    
}