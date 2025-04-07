using Server.Presentation;

namespace ClientServer.Shared.WebSocket
{
    public class SerializableOrder
    {
        public Guid Id { get; set; }
        public SerializableHero Buyer { get; set; }
        public List<SerializableItem> ItemsToBuy { get; set; }

        public SerializableOrder()
        {
            Id = Guid.NewGuid();
            Buyer = new SerializableHero();
            ItemsToBuy = new List<SerializableItem>();
        }

        public SerializableOrder(Guid id, SerializableHero buyer, List<SerializableItem> itemsToBuy)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy;
        }
    }
}
