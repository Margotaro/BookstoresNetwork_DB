using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
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


        //UPDATE
        public void updateWorker(Worker worker)
        {
            string name = worker.name;
            string id = worker.id.huelue;
            string salary = (worker.salary).ToString("F2");
            string position = ((Position)worker.position).ToString();
            string login = worker.login;
            string hours = worker.hours.shiftname;

            throw new NotImplementedException();
            string QueryString = "not implemented yet";
            //string QueryString = "UPDATE staff SET staff.employee_id = " + id + ", staff.full_name = " + name + ", staff." + "CURRENT_DATE,NULL," + position + "," + salary + "," + hours + "," + "1);";
            sendRequest(QueryString);
        }
        public void updateBook(Book book)
        {

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
        }
        //DELETE
        public void deleteBook(Id<IBook> book_id)
        {
            var queryString = "delete from books where book_id = " + book_id.ToString();
            sendRequest(queryString);
            cachedBooks.Remove(book_id);
        }

        //MODIFICATION
        public void fireWorker(Id<IWorker> worker_id, DateTime firingDate)
        {
            var queryString = "update staff set dehiring_date = " + "\'" + firingDate.Year.ToString() + "-" + firingDate.Month.ToString() + "-" + firingDate.Day.ToString() + "\'" + " where employee_id = " + worker_id.ToString();
            sendRequest(queryString);
            var tempRecachedWorkers = getAllWorkers();
        }

        //SELECTION
        public IEnumerable<Book> findBooks(string title, string author, string publisher, string language, IEnumerable<string> genres)
        {
            var bookList = new List<Book>();

            var queryString = @"SELECT * FROM BOOKS 
                                LEFT JOIN PRINTED_BOOKS
                                ON BOOKS.BOOK_ID = PRINTED_BOOKS.BOOK_ID
                                LEFT JOIN ELECTRONIC_BOOKS
                                ON BOOKS.BOOK_ID = ELECTRONIC_BOOKS.BOOK_ID";
            if(title != "" || author != "" || publisher != "" || language != "" || genres.Count() > 0)
            {
                var isAndNeeded = false;
                queryString += " WHERE ";
                if(title != "")
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    queryString += " BOOKS.TITLE ILIKE " + "'" + title + "'";//ILIKE - ignoring case (lower upper) operator
                }
                if(genres.Count() > 0)
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    var querygenre = convertListToGenresQuery(genres);
                    queryString += " BOOKS.GENRES @> " + querygenre + " AND BOOKS.GENRES @> " + querygenre;
                }
                if(author != "")
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    queryString += " BOOKS.AUTHOR ILIKE " + "'" + author + "'";
                }
                if (publisher != "")
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    queryString += " BOOKS.PUBLISHER ILIKE " + "'" + publisher + "'";
                }
                if (language != "")
                {
                    if (isAndNeeded)
                        queryString += " AND ";
                    else
                        isAndNeeded = true;
                    queryString += " BOOKS.LANGUAGE ILIKE " + "'" + language + "'";
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
        string convertListToGenresQuery(IEnumerable<string> list)
        {
            var _genres = new List<string>();
            foreach (var gen in list)
            {
                _genres.Add("'" + gen.ToString() + "'");
            }
            var arrayGenresString = String.Join(", ", _genres);
            return arrayGenresString;
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

}
