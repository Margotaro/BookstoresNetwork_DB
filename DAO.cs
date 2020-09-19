using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Npgsql;

namespace BookStore
{
    class DAO : IDAO
    {
        private static Dictionary<Id<IBookstore>, Bookstore> cachedBookstores;
        private static Dictionary<Id<IWorker>, Worker> cachedWorkers;
        private static Dictionary<Id<IBook>, Book> cachedBooks;

        private static Dictionary<Id<IBookstore>, List<Tuple<Id<IBook>, int>>> cachedAvailableBooksInBookstore;

        public List<Bookstore> CachedBookstores 
        { 
            get 
            { 
                return cachedBookstores.Select(p => p.Value).ToList(); 
            } 
        }
        public List<Worker> CachedWorkers 
        { 
            get 
            { 
                return cachedWorkers.Select(p => p.Value).ToList();
            } 
        }
        public List<Book> CachedBooks
        {
            get
            {
                return cachedBooks.Select(p => p.Value).ToList();
            }
        }

        public Dictionary<Id<IBookstore>, List<Tuple<Id<IBook>, int>>> CachedAvailableBooksInBookstore
        {
            get
            {
                return cachedAvailableBooksInBookstore;
            }
        }

        public Dictionary<Id<IBook>, List<Tuple<Id<IBookstore>, int>>> CachedBookstoresOnBooksAvailability
        {
            get
            {
                var dict_converted = new Dictionary<Id<IBook>, List<Tuple<Id<IBookstore>, int>>>();
                foreach (var entry in cachedAvailableBooksInBookstore)
                {
                    foreach (var t_book_q in entry.Value)
                    {
                        if (dict_converted.ContainsKey(t_book_q.Item1))
                        {
                            dict_converted[t_book_q.Item1].Add(new Tuple<Id<IBookstore>, int>(entry.Key, t_book_q.Item2));
                        }
                        else
                        {
                            dict_converted.Add(t_book_q.Item1, new List<Tuple<Id<IBookstore>, int>> { new Tuple<Id<IBookstore>, int>(entry.Key, t_book_q.Item2) });
                        }
                    }
                }
                return dict_converted;
            }
        }
        private DatabaseConnection _sqlconnection;
        private WorkerFactory workerFactory;
        private WorkerChangeManager workerChangeManager;
        private BookstoreFactory bookstoreFactory;
        private BookstoreChangeManager bookstoreChangeManager;
        private BookChangeManager bookChangeManager;
        private BookFactory bookFactory;

        public DAO(IBookstoreWindow window)//добавить разные уровни соединения
        {
            _sqlconnection = ConnectAdminRoute();
            
            workerChangeManager = new WorkerChangeManager(this, window);
            bookstoreChangeManager = new BookstoreChangeManager(this);
            bookChangeManager = new BookChangeManager(this, window);
            
            workerFactory = new WorkerFactory(workerChangeManager, this);
            bookstoreFactory = new BookstoreFactory(bookstoreChangeManager);
            bookFactory = new BookFactory(bookChangeManager);

            cachedAvailableBooksInBookstore = new Dictionary<Id<IBookstore>, List<Tuple<Id<IBook>, int>>>();
            getAllBookstores();
            getAllWorkers();
            getCatalogueContents();
        }

        //GET
        public IEnumerable<Book> getCatalogueContents()
        {
            var bookList = new List<Book>();
            var queryString = @"SELECT * FROM BOOKS 
                                LEFT JOIN PRINTED_BOOKS
                                ON BOOKS.BOOK_ID = PRINTED_BOOKS.BOOK_ID
                                LEFT JOIN ELECTRONIC_BOOKS
                                ON BOOKS.BOOK_ID = ELECTRONIC_BOOKS.BOOK_ID";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            foreach (var entry in reader)
            {
                var _electronic_ver_cost = reader["electronic_ver_cost"].ToString() == "" ? "0" : reader["electronic_ver_cost"].ToString();
                var _retail_printed_ver_cost = reader["cost_retail"].ToString() == "" ? "0" : reader["cost_retail"].ToString();
                var _wholesail_printed_ver_cost = reader["cost_wholesale"].ToString() == "" ? "0" : reader["cost_wholesale"].ToString();
                var _warehouse_printed_ver_quantity = reader["quantity_at_warehouse"].ToString() == "" ? "0" : reader["quantity_at_warehouse"].ToString();

                var _genres_list = reader["genres"] as IEnumerable;
                bookList.Add((Book)bookFactory?.makeBook(
                                reader["book_id"].ToString(),
                                reader["title"].ToString(),
                                reader["author"].ToString(),
                                reader["publisher"].ToString(),
                                reader["lang"].ToString(),
                                _genres_list,
                                Convert.ToDouble(_electronic_ver_cost),
                                reader["pdf_version"].ToString(),
                                Convert.ToDouble(_retail_printed_ver_cost),
                                Convert.ToDouble(_wholesail_printed_ver_cost),
                                Convert.ToInt32(_warehouse_printed_ver_quantity)
                                )); 
            }
            reader.Close();
            cachedBooks = bookList.ToDictionary(x => x.id, x => x);
            updateCataloguePerBookstoreDictionary();
            return bookList;
        }
        public IEnumerable<Bookstore> getAllBookstores()
        {
            var bookstoresList = new List<Bookstore>();
            var queryString = "SELECT * FROM BOOKSTORES";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            foreach (var entry in reader)
            {
                var raw_address = reader["address_id"] as IDictionary<string, object>;
                bookstoresList.Add((Bookstore)bookstoreFactory?.makeBookstore(
                                reader["bookstore_id"].ToString(),
                                raw_address["city"].ToString(),
                                raw_address["street"].ToString(),
                                raw_address["house"].ToString(),
                                reader["cashier_telephone"].ToString(),
                                reader["manager_telephone"].ToString()
                                ));
            }
            reader.Close();
            cachedBookstores = bookstoresList.ToDictionary(x => x.id, x => x);
            return bookstoresList;
        }
        public IEnumerable<Worker> getAllWorkers()
        {
            var workersList = new List<Worker>();
            var queryString = "SELECT * FROM STAFF";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            foreach (var entry in reader)
            {
                System.DateTime? hiringdate = null;
                System.DateTime? dehiringdate = null;
                if(!reader.IsDBNull(2))
                    hiringdate = reader.GetDateTime(2);
                if (!reader.IsDBNull(3))
                    dehiringdate = reader.GetDateTime(3);

                string hiringdate_year = "0";
                string hiringdate_month = "0";
                string hiringdate_day = "0";

                string dehiringdate_year = "0";
                string dehiringdate_month = "0";
                string dehiringdate_day = "0";

                if (hiringdate != null)
                {
                    hiringdate_year = hiringdate?.Year.ToString();
                    hiringdate_month = hiringdate?.Month.ToString();
                    hiringdate_day = hiringdate?.Day.ToString();
                }

                if (dehiringdate != null)
                {
                    dehiringdate_year = dehiringdate?.Year.ToString();
                    dehiringdate_month = dehiringdate?.Month.ToString();
                    dehiringdate_day = dehiringdate?.Day.ToString();
                }
                workersList.Add((Worker)workerFactory?.makeWorker(
                                reader["full_name"].ToString(),
                                reader["job_type"].ToString(),//??
                                reader["salary"].ToString(),
                                reader["shift_id"].ToString(),
                                reader["bookstore_id"].ToString(),
                                "unimplemented login",
                                "unimplemented password",
                                hiringdate_year,
                                hiringdate_month,
                                hiringdate_day,
                                reader["employee_id"].ToString(),
                                dehiringdate_year,
                                dehiringdate_month,
                                dehiringdate_day
                                ));
            }
            reader.Close();
            cachedWorkers = workersList.ToDictionary(x => x.id, x => x);
            return workersList;
        }
        public Bookstore getBookstore(Id<IBookstore> bookstoreId)
        {
            if (cachedBookstores.ContainsKey(bookstoreId))
            {
                return cachedBookstores[bookstoreId];
            }

            var queryString = "SELECT * FROM BOOKSTORES WHERE bookstores.bookstore_id = " + bookstoreId.huelue;
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            Bookstore bookstore = null;
            if(reader.HasRows)
            {
                var raw_address = reader["address_id"] as IDictionary<string, object>;
                bookstore = (Bookstore)bookstoreFactory?.makeBookstore(
                                reader["bookstore_id"].ToString(),
                                raw_address["city"].ToString(),
                                raw_address["street"].ToString(),
                                raw_address["house"].ToString(),
                                reader["cashier_telephone"].ToString(),
                                reader["manager_telephone"].ToString()
                                );
            }
            reader.Close();
            cachedBookstores[bookstoreId] = bookstore;
            return bookstore;
        }
        private void updateCataloguePerBookstoreDictionary()
        {
            var queryString = @"SELECT BOOKSTORES.BOOKSTORE_ID, ARRAY_AGG(BOOKS_FOR_SALE.BOOK_ID || ', ' ||BOOKS_FOR_SALE.QUANTITY)  FROM BOOKS
                                JOIN PRINTED_BOOKS
                                ON BOOKS.BOOK_ID = PRINTED_BOOKS.BOOK_ID
                                JOIN BOOKS_FOR_SALE
                                ON PRINTED_BOOKS.BOOK_ID = BOOKS_FOR_SALE.BOOK_ID
                                JOIN BOOKSTORES
                                ON BOOKSTORES.BOOKSTORE_ID = BOOKS_FOR_SALE.BOOKSTORE_ID
                                WHERE BOOKS_FOR_SALE.QUANTITY != 0
                                GROUP BY BOOKSTORES.BOOKSTORE_ID";

            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                cachedAvailableBooksInBookstore = new Dictionary<Id<IBookstore>, List<Tuple<Id<IBook>, int>>>();
            }
            foreach (var entry in reader)
            {
                var item = reader["array_agg"] as IEnumerable<string>;
                int count = 0;
                foreach(var i in item)
                {
                    count += 1;
                }
                if(count > 0)
                {
                    var list_tuple = new List<Tuple<Id<IBook>, int>>();
                    foreach(var raw_string_bookid_quantity in item)
                    {
                        var string_bookid_quantity = raw_string_bookid_quantity.Split(',').ToList();
                        list_tuple.Add(new Tuple<Id<IBook>, int>(new Id<IBook>(string_bookid_quantity.ElementAt(0)), Convert.ToInt32(string_bookid_quantity.ElementAt(1))));
                    }
                    var _t = new Id<IBookstore>(reader["bookstore_id"].ToString());
                    if(cachedAvailableBooksInBookstore.ContainsKey(_t))
                    {
                        foreach(var element in list_tuple)
                        {
                            cachedAvailableBooksInBookstore[_t].Add(element);
                        }
                    }
                    else
                    {
                        cachedAvailableBooksInBookstore.Add(new Id<IBookstore>(reader["bookstore_id"].ToString()), list_tuple);
                    }
                    
                }
            }

            reader.Close();
        }

        //UPDATE
        public void updateWorker(Worker worker_update)
        {
            var queryString = @"UPDATE staff 
                                SET full_name = '" + worker_update.name + "', " +
                                   "job_type = '" + worker_update.position.ToString() + "', " +
                                   "salary = '" + worker_update.salary.ToString() + "', " +
                                   "shift_id = '" + worker_update.hours.shiftname + "' " +
                                "WHERE staff.employee_id = " + worker_update.id.ToString();
            sendRequest(queryString);
            getAllWorkers();
        }
        public void updateBook(Book book)
        {

            updateCataloguePerBookstoreDictionary();
        }



        //CREATE
        public void createWorker(string name, double salary, Position position, string login, string password, WorkingHours hours, DateTime hiring_date, Bookstore working_place)
        {
            foreach(var worker in cachedWorkers)
            {
                if(worker.Value.name == name && 
                   worker.Value.position.ToString() == position.ToString() &&
                   worker.Value.hiring_date.ToString() == hiring_date.ToString())
                { 
                    return; 
                }
            }
            var _name = name;
            var _salary = salary.ToString();
            var _position = position.ToString();
            var _login = login;
            var _password = password;
            var _hours = hours.shiftname;
            var _hiringdate = hiring_date.ToString();
            var _bookstoreid = working_place.id.ToString();

            var queryString = "INSERT INTO \"staff\" VALUES (DEFAULT,'" + _name + "'," + "'" + _hiringdate + "'" + ",NULL,'" + _position + "'," + _salary + ",'" + _hours + "'," + _bookstoreid + ");";
            sendRequest(queryString);

            queryString = "SELECT employee_id FROM staff ORDER BY employee_id DESC limit 1";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            var worker_id = new Id<IWorker>("-1");
            if (reader.HasRows)
            {
                reader.Read();
                worker_id = new Id<IWorker>(reader["employee_id"].ToString());
            }
            else
            { 
                throw new Exception("Workers db are empty"); 
            }
            reader.Close();

            cachedWorkers.Add(worker_id, (Worker)workerFactory.makeWorker(_name, _position, _salary, _hours, _bookstoreid, _login, _password, hiring_date.Year.ToString(), hiring_date.Month.ToString(), hiring_date.Day.ToString(), worker_id.ToString())); 
        }

        public void createBook(string title, string author, string publisher, string language, IEnumerable genres, 
                               string electronic_ver_cost, string electronic_ver_storage_route, 
                               string retail_printed_ver_cost, string wholesail_printed_ver_cost, 
                               string warehouse_printed_ver_quantity)
        {
            foreach(var book in CachedBooks)
            {
                if(book.title == title && book.author == author && book.publisher == publisher && book.language == language)
                { return; }
            }

            var arrayGenresString = convertListToGenresQuery(genres);
            var queryString = "INSERT INTO \"books\" values(DEFAULT, '" + title + "', '" + author + "', '" + publisher + "', '" + language + "', ARRAY[" + arrayGenresString + "]::genre[]);";
            sendRequest(queryString);

            var book_id = new Id<IBook>(findLastIndexOfDBObject("books", "book_id"));


            var _electronic_ver_cost = 0.0;
            var _retail_printed_ver_cost = 0.0;
            var _wholesail_printed_ver_cost = 0.0;
            var _warehouse_printed_ver_quantity = 0;

            if (electronic_ver_cost != "" && electronic_ver_storage_route != "")
            {
                _electronic_ver_cost = Convert.ToDouble(electronic_ver_cost);
                queryString = "INSERT INTO \"electronic_books\" values(" + book_id.huelue +", $$" + electronic_ver_storage_route + "$$, " + electronic_ver_cost.ToString() + ");";
                sendRequest(queryString);
            }

            if (retail_printed_ver_cost != "" && wholesail_printed_ver_cost != "" && warehouse_printed_ver_quantity != "")
            {
                _retail_printed_ver_cost = Convert.ToDouble(retail_printed_ver_cost);
                _wholesail_printed_ver_cost = Convert.ToDouble(wholesail_printed_ver_cost);
                _warehouse_printed_ver_quantity = Convert.ToInt32(warehouse_printed_ver_quantity);

                queryString = "INSERT INTO \"printed_books\" values(" + book_id.huelue + ", " + wholesail_printed_ver_cost + ", " + retail_printed_ver_cost + ", " + warehouse_printed_ver_quantity + ");";
                sendRequest(queryString);
            }
            cachedBooks.Add(book_id, (Book)bookFactory.makeBook(book_id.ToString(), title, author, publisher, language, genres, _electronic_ver_cost, electronic_ver_storage_route, _retail_printed_ver_cost, _wholesail_printed_ver_cost, _warehouse_printed_ver_quantity));
            updateCataloguePerBookstoreDictionary();
        }
        public Id<IReceipt> purchaseBooks(List<PurchaseParameters> list_of_books_to_purchase)
        {
            try
            {
                var receipt_id = new Id<IReceipt>("-1");
                if (list_of_books_to_purchase.Count == 1)
                {
                    receipt_id = purchaseSingleBook(list_of_books_to_purchase[0]);
                }
                else
                {
                    receipt_id = purchaseSingleBook(list_of_books_to_purchase[0]);
                    for (var i = 1; i < list_of_books_to_purchase.Count; i++)
                    {
                        purchaseSingleBook(list_of_books_to_purchase[i], receipt_id);
                    }
                }
                return receipt_id;
            }
            catch(NpgsqlException ex)
            {
                throw new Exception("bruh");
            }
        }
        private Id<IReceipt> purchaseSingleBook(PurchaseParameters purchaseParameters, Id<IReceipt> receipt_number_id = null)
        {
            var queryString = @"SELECT insert_data_function(" + purchaseParameters.cashier_id.ToString() + ", " +
                                                     purchaseParameters.bookstore_id.ToString() + ", " +
                                                     purchaseParameters.book_id.ToString() + ", " + 
                                                     "'" + purchaseParameters.type_of_the_book + "', " +
                                                     purchaseParameters.quantity.ToString();
            if(receipt_number_id != null)
            {
                queryString += ", " + receipt_number_id.ToString();
            }
            queryString += ")";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            reader.Read();
            var receipt_id = new Id<IReceipt>(reader["insert_data_function"].ToString());
            reader.Close();
            updateCataloguePerBookstoreDictionary();
            return receipt_id;
        }
        //DELETE
        public void deleteBook(Id<IBook> book_id)
        {
            var queryString = "delete from books where book_id = " + book_id.ToString();
            sendRequest(queryString);
            cachedBooks.Remove(book_id);
            updateCataloguePerBookstoreDictionary();
        }
        public void returnBooks(Id<IBook> book_id, int quantity, Id<IReceipt> receipt_id, string book_type)
        {
                var queryString = @"CALL return_book(" + receipt_id.ToString() + ", " + book_id.ToString() + ", " + quantity.ToString() + ", '" + book_type + "')";
                sendRequest(queryString);
                updateCataloguePerBookstoreDictionary();
        }
        //MODIFICATION
        public void fireWorker(Id<IWorker> worker_id, DateTime firingDate)
        {
            var queryString = @"update staff set dehiring_date = " + "\'" + firingDate.Year.ToString() + "-" + firingDate.Month.ToString() + "-" + firingDate.Day.ToString() + "\'" + 
                               " where employee_id = " + worker_id.ToString();
            sendRequest(queryString);
            var tempRecachedWorkers = getAllWorkers();
        }
        //SELECTION
        public IEnumerable<Book> findBooks(string title, string author, string publisher, string language, IEnumerable genres)
        {
            var bookList = new List<Book>();

            var queryString = @"SELECT * FROM BOOKS 
                                LEFT JOIN PRINTED_BOOKS
                                ON BOOKS.BOOK_ID = PRINTED_BOOKS.BOOK_ID
                                LEFT JOIN ELECTRONIC_BOOKS
                                ON BOOKS.BOOK_ID = ELECTRONIC_BOOKS.BOOK_ID";
            var gencount = 0;
            genres = genres == null ? new List<string>() : genres;
            foreach (var t in genres)
            {
                gencount += 1;
            }
            if(title != "" || author != "" || publisher != "" || language != "" || gencount > 0)
            {
                var isAndNeeded = false;
                queryString += " WHERE ";
                if(title != "")
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    queryString += " BOOKS.TITLE ILIKE " + "'%" + title + "%'";//ILIKE - ignoring case (lower upper) operator
                }
                if(gencount > 0)
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    var querygenre = convertListToGenresQuery(genres);
                    queryString += " BOOKS.GENRES @> " + querygenre;
                }
                if(author != "")
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    queryString += " BOOKS.AUTHOR ILIKE " + "'%" + author + "%'";
                }
                if (publisher != "")
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    queryString += " BOOKS.PUBLISHER ILIKE " + "'%" + publisher + "%'";
                }
                if (language != "")
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    queryString += " BOOKS.LANG ILIKE " + "'" + language + "'";
                }
            }
            else
            {
                return CachedBooks;
            }
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            foreach (var entry in reader)
            {
                var _electronic_ver_cost = reader["electronic_ver_cost"].ToString() == "" ? "0" : reader["electronic_ver_cost"].ToString();
                var _retail_printed_ver_cost = reader["cost_retail"].ToString() == "" ? "0" : reader["cost_retail"].ToString();
                var _wholesail_printed_ver_cost = reader["cost_wholesale"].ToString() == "" ? "0" : reader["cost_wholesale"].ToString();
                var _warehouse_printed_ver_quantity = reader["quantity_at_warehouse"].ToString() == "" ? "0" : reader["quantity_at_warehouse"].ToString();

                var _genres_list = reader["genres"] as IEnumerable;
                bookList.Add((Book)bookFactory?.makeBook(
                                reader["book_id"].ToString(),
                                reader["title"].ToString(),
                                reader["author"].ToString(),
                                reader["publisher"].ToString(),
                                reader["lang"].ToString(),
                                _genres_list,
                                Convert.ToDouble(_electronic_ver_cost),
                                reader["pdf_version"].ToString(),
                                Convert.ToDouble(_retail_printed_ver_cost),
                                Convert.ToDouble(_wholesail_printed_ver_cost),
                                Convert.ToInt32(_warehouse_printed_ver_quantity)
                                ));
            }
            reader.Close();
            cachedBooks = bookList.ToDictionary(x => x.id, x => x);
            return bookList;
        }
        private string findLastIndexOfDBObject(string tablename, string idname)
        {
            var queryString = "SELECT " + idname + " FROM " + tablename + " ORDER BY " + idname + " DESC limit 1";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            var book_id = "";
            if (reader.HasRows)
            {
                reader.Read();
                book_id = reader["book_id"].ToString();
            }
            else
            {
                throw new Exception("Books db are empty");
            }
            reader.Close();
            return book_id;
        }
        private void UpdateData()//call when db data changes 
        {

        }
        public IEnumerable<string> uploadGenres()
        {
            var queryString = "SELECT enum_range(NULL::genre)";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                //genres = reader["enum_range"].ToString().Split(new char[] {',', '{', '}'}).ToList();
                var genres = reader["enum_range"] as IEnumerable<string>;
                reader.Close();
                return genres;
            }
            else
            {
                throw new Exception("Books db are empty");
            }
        }
        string convertListToGenresQuery(IEnumerable list)
        {
            var _genres = new List<string>();
            foreach (var gen in list)
            {
                _genres.Add("'" + gen.ToString() + "'");
            }
            var arrayGenresString = String.Join(", ", _genres);
            return "ARRAY[" + arrayGenresString + "]::genre[]";
        }
        private void sendRequest(string querystring)
        {
            try
            {
                NpgsqlCommand Command = new NpgsqlCommand(querystring, _sqlconnection.CreateConnection.Connection);
                try
                {
                    Command.ExecuteNonQuery();
                }
                catch (PostgresException ex)
                {
                    MessageBox.Show("Ошибка ввода" + Convert.ToString(ex));
                }
            }
            catch (PostgresException ex)
            {
                MessageBox.Show("Ошибка базы данных \n" + Convert.ToString(ex));
            }
        }

        private DatabaseConnection ConnectAdminRoute()
        {
            string ConnectionString = "Host=127.0.0.1;Port=5432;Username=allprivileged_admin;Password=superadmin;Database=bookstores_network";
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection(ConnectionString);
            DatabaseConnection databaseConnection = new DatabaseConnection(npgsqlConnection);
            databaseConnection.OpenConnection();
            return databaseConnection;
        }
    }
    public class PurchaseParameters
    {
        public PurchaseParameters(Id<IWorker> cashier_id, Id<IBookstore> bookstore_id, Id<IBook> book_id, string type_of_the_book, int quantity)
        {
            this.cashier_id = cashier_id;
            this.bookstore_id = bookstore_id;
            this.book_id = book_id;
            this.type_of_the_book = type_of_the_book;
            this.quantity = quantity;
        }
        public Id<IWorker> cashier_id;
        public Id<IBookstore> bookstore_id;
        public Id<IBook> book_id;
        public string type_of_the_book;
        public int quantity;
    }

}
