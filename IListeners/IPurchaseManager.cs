using System.Collections.Generic;

namespace BookStore
{
    public partial class CashierWindow
    {
        public interface IPurchaseManager
        {
            string MakePurchase(List<PurchaseParameters> pp);
            void ReturnBook(Id<IBook> book_id, int quantity, Id<IReceipt> receipt_id);
        }
        public class PurchaseManager
        {
            public PurchaseManager(IDAO dao, IBookstoreWindow window)
            {
                this.dao = dao;
                this.window = window;
            }

            IDAO dao;
            IBookstoreWindow window;

            public string MakePurchase(List<PurchaseParameters> pp)
            {
                var t = dao.purchaseBooks(pp).ToString();
                window.dataGridBookUpdate();
                return t;
            }
            public void ReturnBook(Id<IBook> book_id, int quantity, Id<IReceipt> receipt_id, string book_type)
            {
                dao.returnBooks(book_id, quantity, receipt_id, book_type);
                window.dataGridBookUpdate();
            }
        }
    }
}
