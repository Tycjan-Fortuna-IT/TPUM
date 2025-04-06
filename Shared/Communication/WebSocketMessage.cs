using System;
using System.Xml.Serialization;

namespace Shared.Communication
{
    [Serializable]
    [XmlRoot("Message")] // Define the root element name for the overall message
    // Include known types for all possible payload types
    [XmlInclude(typeof(Shared.Models.Hero))]
    [XmlInclude(typeof(Shared.Models.Item))]
    [XmlInclude(typeof(Shared.Models.Inventory))]
    [XmlInclude(typeof(Shared.Models.Order))]
    [XmlInclude(typeof(Shared.Models.Hero[]))] // For responses returning collections
    [XmlInclude(typeof(Shared.Models.Item[]))]
    [XmlInclude(typeof(Shared.Models.Inventory[]))]
    [XmlInclude(typeof(Shared.Models.Order[]))]
    [XmlInclude(typeof(string))] // For error messages or simple confirmations
    public class WebSocketMessage
    {
        public MessageType MessageType { get; set; }

        public object? Payload { get; set; }

        // Parameterless constructor for serialization
        public WebSocketMessage() { }

        public WebSocketMessage(MessageType messageType, object? payload = null)
        {
            MessageType = messageType;
            Payload = payload;
        }
    }
}