using System.Collections;

namespace BookStore
{
    public interface IBook
    {
        Id<IBook> id { get; }
        string title { get; set; }
        string author  { get; set; }
        string publisher { get; set; }
        string language { get; set; }
        IEnumerable genre { get; set; }//enum???
    }
}
