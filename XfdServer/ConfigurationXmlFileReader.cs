using System.IO;
using System.Xml.Serialization;

namespace XfdServer
{
    internal static class ConfigurationXmlFileReader
    {
        public static Projects Read(string filePath)
        {
            var str = new StreamReader(filePath);
            var serializer = new XmlSerializer(typeof(Projects));
            var result = (Projects)serializer.Deserialize(str);
            str.Close();
            return result;
        }
    }
}