using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Shared.DataModels;

namespace Shared.Communication
{
    public static class XmlSerializationHelper
    {
        // Typy, które mogą wystąpić w polach 'Payload' (obiekt)
        private static readonly Type[] KnownPayloadTypes = new Type[] {
            typeof(Hero), typeof(List<Hero>),
            typeof(Item), typeof(List<Item>),
            typeof(Inventory),
            typeof(Order), typeof(List<Order>),
            typeof(Guid), typeof(bool),
            typeof(UpdateHeroPayload)
            // Dodać wszystkie inne typy, które mogą być w Payload
        };

        public static string Serialize<T>(T obj) where T : class
        {
            if (obj == null) return string.Empty;

            var serializer = new XmlSerializer(typeof(T), KnownPayloadTypes);
            using (var stringWriter = new StringWriter())
            {
                // Usunięcie deklaracji XML i przestrzeni nazw dla zwięzłości
                var settings = new System.Xml.XmlWriterSettings
                {
                    Indent = false,
                    OmitXmlDeclaration = true
                };
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                using (var xmlWriter = System.Xml.XmlWriter.Create(stringWriter, settings))
                {
                    serializer.Serialize(xmlWriter, obj, ns);
                }
                return stringWriter.ToString();
            }
        }

        public static T? Deserialize<T>(string xml) where T : class
        {
            if (string.IsNullOrEmpty(xml)) return default;

            var serializer = new XmlSerializer(typeof(T), KnownPayloadTypes);
            using (var stringReader = new StringReader(xml))
            {
                try
                {
                    return serializer.Deserialize(stringReader) as T;
                }
                catch (InvalidOperationException ex) // Błąd deserializacji
                {
                    Console.WriteLine($"XML Deserialization Error: {ex.Message} - XML: {xml}");
                    return default;
                }
            }
        }
    }
}