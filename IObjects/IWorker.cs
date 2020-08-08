namespace BookStore
{
    interface IWorker
    {
        string name { get; set; }
        Position position { get; set; }
        double salary { get; set; }
        WorkingHours hours { get; set; }
        string login { get; set; }
        string password { get; set; } 
        Id<IWorker> id { get; set; } 
        Date hiring_date { get; set; }
        Date dehiring_date { get; set; }
        Bookstore working_place { get; set; }
}
}
