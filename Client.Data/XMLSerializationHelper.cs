using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Shared.Communication; // Potrzebne dla WebSocketMessage i typów z [XmlInclude]

namespace Client.Data
{
    // Skopiuj kod klasy XmlSerializationHelper z Server.Data
    public static class XmlSerializationHelper
    {
        // Serializuje obiekt do stringa XML
        public static string Serialize<T>(T obj) where T : class
        {
            if (obj == null) return string.Empty;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriter stringWriter = new StringWriter())
                using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.None }) // użyte None dla mniejszego rozmiaru
                {
                    serializer.Serialize(xmlWriter, obj);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Serialization Error] Type: {typeof(T).Name}, Error: {ex.Message}");
                return string.Empty;
            }
        }

        // Deserializuje string XML do obiektu
        public static T? Deserialize<T>(string xml) where T : class
        {
            if (string.IsNullOrEmpty(xml)) return null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringReader stringReader = new StringReader(xml))
                {
                    return serializer.Deserialize(stringReader) as T;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Deserialization Error] Target Type: {typeof(T).Name}, Error: {ex.Message}, XML: {xml.Substring(0, Math.Min(xml.Length, 100))}");
                // TODO: Lepsze logowanie błędów
                return null; // Zwróć null lub rzuć wyjątek
            }
        }

        // Metoda specyficzna dla WebSocketMessage
        public static Generated.WebSocketMessage? DeserializeMessage(string xml)
        {
            return Deserialize<Generated.WebSocketMessage>(xml);
        }
        public static string SerializeMessage(Generated.WebSocketMessage message)
        {
            return Serialize(message);
        }
    }
}
