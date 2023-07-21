namespace Models
{
    public class Basket
    {
        public string BasketId { get; set; }
        public IEnumerable<string> ConcertIds { get; set; }

        public Basket(string basketId, IEnumerable<string> concertIds)
        {
            BasketId = basketId;
            ConcertIds = concertIds;
        }
    }
}