using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace vocab_tester
{
    public class DictionaryXMLHelper
    {
        public class QuestionXML
        {
            public string category { get; set; }
            public string question { get; set; }
            public List<string> answers { get; set; }

            public QuestionXML(string category, string question)
            {
                this.category = category;
                this.question = question;
                answers = new List<string>();
            }

            public void AddAnswer(string answer)
            {
                answers.Add(answer);
            }
        }
        public class DB_info
        {
            public string version { get; set; }
            public string file_id { get; set; }

            public string file_link { get; set; }

            public DB_info()
            {
                version = "";
                file_id = "";
                file_link = "";
            }
        }

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
                [XmlAttribute]
                public string id { get; set; }

                [XmlAttribute]
                public string link { get; set; }
            }
        }

        public async Task<DB_info> DownloadVersion()
        {
            DB_info db_info = new DB_info();
            string page = "http://drive.google.com/uc?export=download&id=1Em6g-Yvgb-eDboBBtK_phXe0OCqDNjZO";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                System.IO.Stream xml_stream = await content.ReadAsStreamAsync();

                XmlSerializer ser = new XmlSerializer(typeof(DictionaryXML_info.Info));
                DictionaryXML_info.Info info;
                using (XmlReader reader = XmlReader.Create(xml_stream))
                {
                    info = (DictionaryXML_info.Info)ser.Deserialize(reader);
                }

                db_info.version = info.version.number;
                db_info.file_id = info.file.id;
                db_info.file_link = info.file.link;
                return db_info;
            }
        }

        public async Task<XmlDocument> DownloadDictionary(string file_link)
        {
            string page = string.Format("http://drive.google.com/uc?export=download&id={0}", file_link);
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                string xml_string = await content.ReadAsStringAsync();
                XmlDocument xml_doc = new XmlDocument();
                xml_doc.LoadXml(xml_string);
                return xml_doc;
            }
        }
    }
}
    
