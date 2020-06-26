namespace BookStore
{
    interface IBook
    {
        string title { get; set; }
        string author  { get; set; }
        string publisher { get; set; }
        string language { get; set; }
        string genre { get; set; }
    }
}
