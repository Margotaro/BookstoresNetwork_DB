using System.Collections;

public class BookViewModel
{
    public string title;
    public string author;
    public string publisher;
    public string language;
    public IEnumerable genre;
    public string electronic_ver_cost;
    public string electronic_ver_storage_route;
    public string retail_printed_ver_cost;
    public string wholesail_printed_ver_cost;
    public string warehouse_printed_ver_quantity;

    public BookViewModel(string title, string author, string publisher, string language, IEnumerable genre, 
                         string electronic_ver_cost, string electronic_ver_storage_route, string retail_printed_ver_cost, 
                         string wholesail_printed_ver_cost, string warehouse_printed_ver_quantity)
    {
        this.title = title;
        this.author = author;
        this.publisher = publisher;
        this.language = language;
        this.genre = genre;
        this.electronic_ver_cost = electronic_ver_cost;
        this.electronic_ver_storage_route = electronic_ver_storage_route;
        this.retail_printed_ver_cost = retail_printed_ver_cost;
        this.wholesail_printed_ver_cost = wholesail_printed_ver_cost;
        this.warehouse_printed_ver_quantity = warehouse_printed_ver_quantity;
    }
}