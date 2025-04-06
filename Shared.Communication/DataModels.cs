using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shared.DataModels
{
    // Podstawowe modele danych - uproszczone wersje interfejsów
    // Atrybuty Xml... mogą być potrzebne do dostosowania serializacji

    [Serializable]
    public class Hero
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Gold { get; set; }
        public Inventory? Inventory { get; set; } // Może być null jeśli nie zawsze jest powiązany
    }

    [Serializable]
    public class Inventory
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
    }

    [Serializable]
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int MaintenanceCost { get; set; }
    }

    [Serializable]
    public class Order
    {
        public Guid Id { get; set; }
        public Guid BuyerHeroId { get; set; } // Przesyłamy tylko ID, pełny obiekt zrekonstruujemy na serwerze/kliencie
        public List<Guid> ItemIdsToBuy { get; set; } = new List<Guid>(); // Przesyłamy tylko ID
        // Jeśli potrzebujesz pełnych obiektów w XML, zmień Guid na odpowiednie klasy (Hero, Item)
        // ale to zwiększy rozmiar wiadomości. Zazwyczaj wystarczą ID.
    }

    // --- Struktury do komunikacji ---

    [Serializable]
    public enum OperationType
    {
        // Hero Operations
        GetAllHeroes,
        GetHeroById,
        AddHero,
        UpdateHero,
        RemoveHero,
        // Item Operations
        GetAllItems,
        GetItemById,
        AddItem,
        UpdateItem,
        RemoveItem,
        // Order Operations
        GetAllOrders,
        GetOrderById,
        AddOrder,
        UpdateOrder, // Zazwyczaj zamówienia się realizuje, a nie aktualizuje, ale dodaję dla spójności
        RemoveOrder,
        // Można dodać inne operacje np. GetInventoryById
        Error, // Do sygnalizowania błędów
        Success // Do potwierdzeń bez danych
    }

    [Serializable]
    [XmlRoot("Request")]
    // Klasa bazowa dla różnych typów payloadów
    [XmlInclude(typeof(Hero))]
    [XmlInclude(typeof(Item))]
    [XmlInclude(typeof(Order))]
    [XmlInclude(typeof(Guid))] // Dla operacji Get/Remove by ID
    [XmlInclude(typeof(UpdateHeroPayload))] // Przykład dla złożonych operacji
    // ... dodaj inne typy payloadów jeśli są potrzebne
    public class Request
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();
        public OperationType Operation { get; set; }
        public object? Payload { get; set; } // Używamy object, aby pomieścić różne typy danych
    }

    // Przykład złożonego payloadu dla aktualizacji
    [Serializable]
    public class UpdateHeroPayload
    {
        public Guid HeroId { get; set; }
        public Hero? HeroData { get; set; }
    }


    [Serializable]
    [XmlRoot("Response")]
    // Klasa bazowa dla różnych typów payloadów w odpowiedzi
    [XmlInclude(typeof(List<Hero>))]
    [XmlInclude(typeof(Hero))]
    [XmlInclude(typeof(List<Item>))]
    [XmlInclude(typeof(Item))]
    [XmlInclude(typeof(List<Order>))]
    [XmlInclude(typeof(Order))]
    [XmlInclude(typeof(bool))] // Dla operacji Remove/Update zwracających sukces/porażkę
    // ... dodaj inne typy payloadów odpowiedzi
    public class Response
    {
        public Guid CorrelationId { get; set; } // Powiązanie z RequestId
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public object? Payload { get; set; } // Wynik operacji
    }
}