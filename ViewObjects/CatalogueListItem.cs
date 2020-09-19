using System.Windows.Documents;

public class CatalogueListItem
{
    public string CatalogueID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Publisher { get; set; }
    public string Language { get; set; }
    public string Genres { get; set; }

    public string Electronic_version_cost { get; set; }
    public string Electronic_version_path { get; set; }
    public string Retail_cost { get; set; }
    public string Wholesale_cost { get; set; }
    public string Warehouse_quantity { get; set; }

    public string Bookstores { get; set; }

    public CatalogueListItem(string id, string title, string author, string publisher, string language, string genres, 
                             string electronic_version_cost, string electronic_version_path, string wholesale_cost, 
                             string retail_cost, string warehouse_quantity, string bookstores )
    {
        CatalogueID = id;
        Title = title;
        Author = author;
        Publisher = publisher;
        Language = language;
        Genres = genres;
        Electronic_version_cost = electronic_version_cost;
        Electronic_version_path = electronic_version_path;
        Wholesale_cost = wholesale_cost;
        Retail_cost = retail_cost;
        Warehouse_quantity = warehouse_quantity;
        Bookstores = bookstores;
    }
}
