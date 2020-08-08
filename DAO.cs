using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Windows;
using System.Windows.Documents;
using Npgsql;

namespace BookStore
{
    class DAO : IDAO
    {
        private DatabaseConnection _sqlconnection;
        public DAO(DatabaseConnection sqlconnection)
        {
            _sqlconnection = sqlconnection;
        }
        public IEnumerable<Book> getCatalogueContents()
        {
            var bookList = new List<Book>();
            var queryString = "SELECT * FROM BOOKS";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            var changeManager = new BookChangeManager(this);
            var bookFactory = new BookFactory(changeManager);
            foreach (var entry in reader)
            {
                bookList.Add((Book)bookFactory?.makeBook(
                                reader["book_id"].ToString(),
                                reader["title"].ToString(),
                                reader["author"].ToString(),
                                reader["publisher"].ToString(),
                                reader["lang"].ToString(),
                                reader["genres"].ToString()
                                ));
            }
            reader.Close();
            return bookList;
        }
        public IEnumerable<Bookstore> getAllBookstores()
        {
            var bookstoresList = new List<Bookstore>();
            var queryString = "SELECT * FROM BOOKSTORES";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            var changeManager = new BookstoreChangeManager(this);
            var bookstoreFactory = new BookstoreFactory(changeManager);
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
            return bookstoresList;
        }
        public IEnumerable<Worker> getAllWorkers()
        {
            var workersList = new List<Worker>();
            var queryString = "SELECT * FROM STAFF";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            var changeManager = new WorkerChangeManager(this);
            var workerFactory = new WorkerFactory(changeManager);
            foreach (var entry in reader)
            {
                workersList.Add((Worker)workerFactory?.makeWorker(
                                reader["full_name"].ToString(),
                                reader["job_type"].ToString(),
                                reader["salary"].ToString(),
                                reader["shift_id"].ToString(),
                                "unimplemented login",
                                "unimplemented password",
                                reader["employee_id"].ToString()
                                ));
            }
            reader.Close();
            return workersList;
        }

        private Dictionary<Id<IBookstore>, Bookstore> cachedBookstores;
        public Bookstore getBookstore(Id<IBookstore> bookstoreId)
        {
            if (cachedBookstores.ContainsKey(bookstoreId))
            {
                return cachedBookstores[bookstoreId];
            }

            var queryString = "SELECT * FROM BOOKSTORES WHERE bookstores.bookstore_id = " + bookstoreId.huelue;
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            var changeManager = new BookstoreChangeManager(this);
            var bookstoreFactory = new BookstoreFactory(changeManager);
            Bookstore bookstore = null;
            if(reader.HasRows)
            {
                var raw_address = reader["address_id"].ToString();
                var address = raw_address.Substring(1, raw_address.LastIndexOf(')')).Split(',');
                bookstore = (Bookstore)bookstoreFactory?.makeBookstore(
                                reader["bookstore_id"].ToString(),
                                address[0],
                                address[1],
                                address[2],
                                reader["cashier_telephone"].ToString(),
                                reader["manager_telephone"].ToString()
                                );
            }
            reader.Close();
            cachedBookstores[bookstoreId] = bookstore;
            return bookstore;
        }
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
        public void createWorker(Worker worker)
        {
            string name = worker.name;
            string id = worker.id.huelue;
            string salary = (worker.salary).ToString();// (worker.salary).ToString("F2");
            string position = ((Position)worker.position).ToString();
            string login = worker.login;
            string hours = worker.hours.shiftname;
            
            string QueryString = "INSERT INTO \"staff\" VALUES (" + id + ",'" + name + "'," + "CURRENT_DATE,NULL,'" + position + "'," + salary + ",'" + hours + "'," + "1);";
            MessageBox.Show(QueryString);
            sendRequest(QueryString);
        }

        public void updateBook(Book book)
        {

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
    }

}
