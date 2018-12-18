using System;
using System.Collections.Generic;
using System.Text;

namespace ISIC_Concole_Downloader
{
    class Datasetmetadata
    {
        public int _accessLevel { get; set; }
        public string _id { get; set; }
        public string description { get; set; }
        public string license { get; set; }
        public string name { get; set; }
        public DateTime updated { get; set; }
    }
}
