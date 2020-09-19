namespace BookStore
{
    public interface IReceipt
    {
        Id<IReceipt> id { get; }
        Id<IWorker> cashier_id { get; }
        int cost { get; set; }
        DateTime purchase_timestamp { get; set; }
    }
}
