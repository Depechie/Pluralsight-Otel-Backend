namespace Models
{
    public class BasketItem
    {
        public string ConcertId { get; set; }
        public string BasketId { get; set; }
        public int Quantity { get; set; }

        public BasketItem(string concertId, string basketId, int quantity)
        {
            ConcertId = concertId;
            BasketId = basketId;
            Quantity = quantity;
        }
    }
}