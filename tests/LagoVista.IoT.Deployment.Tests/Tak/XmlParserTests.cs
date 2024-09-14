using LagoVista.IoT.Deployment.Admin.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace LagoVista.IoT.Deployment.Tests.Tak
{
    [TestClass]
    public class XmlParserTests
    {
        public const string MANIFEST_XML = @"<MissionPackageManifest version=""2"">
  <Configuration>
    <Parameter name=""uid"" value=""CAED1C91-B36D-4731-B6A6-2D2CB2666E41""/>
    <Parameter name=""name"" value=""kevin.zip""/>
    <Parameter name=""onReceiveDelete"" value=""false""/>
  </Configuration>
  <Contents>
    <Content ignore=""false"" zipEntry=""certs/config.pref""/>
    <Content ignore=""false"" zipEntry=""certs/truststore-SL-ID-CA-01.p12""/>
    <Content ignore=""false"" zipEntry=""certs/kevinw.p12""/>
  </Contents>
</MissionPackageManifest>";

        public const string PREFENCE_XML = @"<?xml version='1.0' encoding='ASCII' standalone='yes'?>
<preferences>
  <preference version=""1"" name=""cot_streams"">
    <entry key=""count"" class=""class java.lang.Integer"">1</entry>
    <entry key=""description0"" class=""class java.lang.String"">TAK Server</entry>
    <entry key=""enabled0"" class=""class java.lang.Boolean"">true</entry>
    <entry key=""connectString0"" class=""class java.lang.String"">tak.software-logistics.com:8089:ssl</entry>
  </preference>
  <preference version=""1"" name=""com.atakmap.app_preferences"">
    <entry key=""displayServerConnectionWidget"" class=""class java.lang.Boolean"">true</entry>
    
    <entry key=""caLocation"" class=""class java.lang.String"">cert/truststore-SL-ID-CA-01.p12</entry>
    <entry key=""caPassword"" class=""class java.lang.String"">atakatak</entry>
    <entry key=""clientPassword"" class=""class java.lang.String"">atakatak</entry>
    <entry key=""certificateLocation"" class=""class java.lang.String"">cert/kevinw.p12</entry>

    <entry key=""locationCallsign"" class=""class java.lang.String"">bytemaster</entry>
    <entry key=""locationTeam"" class=""class java.lang.String"">Green</entry>
    <entry key=""atakRoleType"" class=""class java.lang.String"">Team Lead</entry>
  </preference>
</preferences>";


        [TestMethod]
        public void ShouldParseManifestXML()
        {
            XmlSerializer serializer = new(typeof(Manifest));

            Console.WriteLine(MANIFEST_XML);

            var stringReader = new StringReader(MANIFEST_XML);

            var result = serializer.Deserialize(stringReader) as Manifest;
            Assert.AreEqual(3, result.Contents.Items.Count);
            Assert.AreEqual(3, result.Configuration.Parameters.Count);
        }


        [TestMethod]
        public void ShouldGetConfigFile()
        {
            XmlSerializer serializer = new(typeof(Manifest));

            Console.WriteLine(MANIFEST_XML);

            var stringReader = new StringReader(MANIFEST_XML);

            var result = serializer.Deserialize(stringReader) as Manifest;
            var prefEntry = result.Contents.Items.FirstOrDefault(cnt => cnt.ZipEntry == "certs/config.pref");
            Assert.IsNotNull(prefEntry);
        }

        [TestMethod]
        public void ShouldFindPreferencesFile()
        {
            XmlSerializer serializer = new(typeof(Manifest));

            Console.WriteLine(MANIFEST_XML);

            var stringReader = new StringReader(MANIFEST_XML);

            var result = serializer.Deserialize(stringReader) as Manifest;
            var prefEntry = result.Contents.Items.FirstOrDefault(cnt => !String.IsNullOrEmpty(cnt.ZipEntry) && cnt.ZipEntry.ToLower().EndsWith("pref"));
            Assert.IsNotNull(prefEntry);
        }

        [TestMethod]
        public void ShouldParsePreferencetXML()
        {
            XmlSerializer serializer = new(typeof(Preferences));

            Console.WriteLine(PREFENCE_XML);

            var stringReader = new StringReader(PREFENCE_XML);

            var result = serializer.Deserialize(stringReader) as Preferences;
            Assert.AreEqual(2, result.Preference.Count);
        }

        [TestMethod]
        public void ShouldGetSteamPropos()
        {
            XmlSerializer serializer = new(typeof(Preferences));

            Console.WriteLine(PREFENCE_XML);

            var stringReader = new StringReader(PREFENCE_XML);

            var result = serializer.Deserialize(stringReader) as Preferences;
            var stream = result.Preference.FirstOrDefault(pref => pref.Name == "cot_streams");
            Assert.AreEqual(4, stream.Entry.Count);

            var connection = stream.Entry.FirstOrDefault(str => str.Key == "connectString0");
            Assert.AreEqual("tak.software-logistics.com:8089:ssl", connection.Text);
        }

        [TestMethod]
        public void ShouldGetPreferencesPropos()
        {
            XmlSerializer serializer = new(typeof(Preferences));

            Console.WriteLine(PREFENCE_XML);

            var stringReader = new StringReader(PREFENCE_XML);

            var result = serializer.Deserialize(stringReader) as Preferences;
            var stream = result.Preference.FirstOrDefault(pref => pref.Name == "com.atakmap.app_preferences");
            Assert.AreEqual(8, stream.Entry.Count);

            var connection = stream.Entry.FirstOrDefault(str => str.Key == "locationTeam");
            Assert.AreEqual("Green", connection.Text);
        }


    }

}
