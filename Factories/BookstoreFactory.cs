using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BookStore
{
    class BookstoreFactory
    {
        public BookstoreFactory(BookstoreChangeManager changeManager)
        {
            this.changeManager = changeManager;
        }
        BookstoreChangeManager changeManager;

        public IBookstore makeBookstore(string Id, string city, string street, string houseNumber, string cashiertelephone, string managertelephone)
        {
            return new Bookstore(changeManager, 
                                    new Id<IBookstore>(Id),
                                    new Address(city, street, Convert.ToInt32(houseNumber)), 
                                    new Telephone(cashiertelephone), 
                                    new Telephone(managertelephone));
        }
    }
}
