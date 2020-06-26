namespace BookStore
{
    interface IWorker
    {
        string name { get; set; }
        Position position { get; set; }
        double salary { get; set; }
        WorkingHours hours { get; set; }
        string login { get; set; }
        string password { get; set; } // int passwordHash?
        Id<IWorker> id { get; set; } // ????
    }
}
