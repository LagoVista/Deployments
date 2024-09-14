using System.Collections.Generic;
using System.Xml.Serialization;

namespace LagoVista.IoT.Deployment.Admin.Services
{

    [XmlRoot(ElementName = "entry")]
    public class Entry
    {

        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }

        [XmlAttribute(AttributeName = "class")]
        public string Class { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "preference")]
    public class Preference
    {

        [XmlElement(ElementName = "entry")]
        public List<Entry> Entry { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public int Version { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "preferences")]
    public class Preferences
    {
        [XmlElement(ElementName = "preference")]
        public List<Preference> Preference { get; set; }
    }


    [XmlRoot("Parameter")]
    public class Parameter
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot("Content")]
    public class Content
    {
        [XmlAttribute(AttributeName = "ignore")]
        public string Ignore { get; set; }


        [XmlAttribute(AttributeName = "zipEntry")]
        public string ZipEntry { get; set; }
    }

    [XmlRoot("Configuration")]
    public class Configuration
    {
        [XmlElement("Parameter")]
        public List<Parameter> Parameters { get; set; }
    }

    [XmlRoot("Contents")]
    public class Contents
    {
        [XmlElement("Content")]
        public List<Content> Items { get; set; }
    }


    [XmlRoot(ElementName = "MissionPackageManifest")]
    public class Manifest
    {
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "Configuration")]
        public Configuration Configuration { get; set; }

        [XmlElement(ElementName = "Contents")]
        public Contents Contents { get; set; }
    }


}
