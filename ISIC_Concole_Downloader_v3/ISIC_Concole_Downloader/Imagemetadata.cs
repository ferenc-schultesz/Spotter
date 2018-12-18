using System;
using System.Collections.Generic;
using System.Text;

namespace ISIC_Concole_Downloader
{
    class Imagemetadata
    {
        public string _id { get; set; }
        public string _modelType { get; set; }
        public DateTime? created { get; set; }
        public Creator creator { get; set; }
        public Dataset dataset { get; set; }
        public Meta meta { get; set; }
        public string name { get; set; }
        public Notes notes { get; set; }
        public DateTime updated { get; set; }

        public class Creator
        {
            public string _id { get; set; }
            public string name { get; set; }
        }

        public class Dataset
        {
            public int? _accessLevel { get; set; }
            public string _id { get; set; }
            public string description { get; set; }
            public string license { get; set; }
            public string name { get; set; }
            public DateTime? updated { get; set; }
        }

        public class Acquisition
        {
            public string image_type { get; set; }
            public int? pixelsX { get; set; }
            public int? pixelsY { get; set; }
        }

        public class Clinical
        {
            public int? age_approx { get; set; }
            public string anatom_site_general { get; set; }
            public string benign_malignant { get; set; }
            public string diagnosis { get; set; }
            public object diagnosis_confirm_type { get; set; }
            public bool? melanocytic { get; set; }
            public string sex { get; set; }
        }

        public class Unstructured
        {
            public string diagnosis { get; set; }
            public string id1 { get; set; }
            public string localization { get; set; }
            public string site { get; set; }
        }

        public class UnstructuredExif
        {
        }

        public class Meta
        {
            public Acquisition acquisition { get; set; }
            public Clinical clinical { get; set; }
            public Unstructured unstructured { get; set; }
            public UnstructuredExif unstructuredExif { get; set; }
        }

        public class Reviewed
        {
            public bool? accepted { get; set; }
            public DateTime? time { get; set; }
            public string userId { get; set; }
        }

        public class Notes
        {
            public Reviewed reviewed { get; set; }
            public List<string> tags { get; set; }
        }
    }
}
