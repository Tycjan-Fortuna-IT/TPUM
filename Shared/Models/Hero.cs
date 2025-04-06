using System;
using System.Xml.Serialization;

namespace Shared.Models
{
    [Serializable]
    public class Hero
    {
        [XmlAttribute]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Gold { get; set; }
        public Inventory? Inventory { get; set; } // Reference to Inventory object

        public Hero() { }

        public Hero(Guid id, string name, float gold, Inventory? inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }
}