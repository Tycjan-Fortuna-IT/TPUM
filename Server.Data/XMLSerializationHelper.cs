using Shared.Communication;
using System.Xml.Serialization;
using System.Xml;

namespace Server.Data;

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
        using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.None }) // Użyte None dla mniejszego rozmiaru, zamienić dla innego formatowania
        {
            serializer.Serialize(xmlWriter, obj);
            return stringWriter.ToString();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Serialization Error] Type: {typeof(T).Name}, Error: {ex.Message}");
        // TODO: Lepsze logowanie błędów
        return string.Empty; // Zwróć pusty string lub rzuć wyjątek
    }
}

// Deserializuje string XML do obiektu
// Używamy typu generycznego T, ale XmlSerializer potrzebuje konkretnego typu.
// Jeśli zawsze serializujemy/deserializujemy WebSocketMessage, możemy uprościć.
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
        return null;
    }
}

// Metoda specyficzna dla WebSocketMessage, aby uniknąć problemów z generykami
public static WebSocketMessage? DeserializeMessage(string xml)
{
    return Deserialize<WebSocketMessage>(xml);
}
public static string SerializeMessage(WebSocketMessage message)
{
    return Serialize(message);
}
}