using System.Windows.Documents;

public class CatalogueListItem
{
    public string CatalogueID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Publisher { get; set; }
    public string Language { get; set; }
    public string Genres { get; set; }

    public CatalogueListItem(string id, string title, string author, string publisher, string language, string genres)
    {
        CatalogueID = id;
        Title = title;
        Author = author;
        Publisher = publisher;
        Language = language;
        Genres = genres;
    }
}
