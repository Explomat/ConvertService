using ConvertService.Models;
using System;
using System.IO;
using System.Xml.Serialization;

namespace ConvertService.Utils
{
    public class XmlSettings
    {
        public static XmlModel LoadXml(string path)
        {
            XmlSerializer reader = new XmlSerializer(typeof(XmlModel));
            StreamReader file = new StreamReader(path);
            XmlModel overview = (XmlModel)reader.Deserialize(file);
            file.Close();
            return overview;
        }
    }
}