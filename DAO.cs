using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public void hui() {
            var queryString = "SELECT full_name FROM STAFF";

            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            foreach (var entry in reader) {
                Console.WriteLine(reader.GetString(0));
            }
        }

        public Bookstore getBookstore(Id<Bookstore> id)
        {
            return new Bookstore(new BookstoreChangeManager(this), new Address("", "", 0), new Telephone(""), new Telephone(""));
        }
        public List<Worker> getWorkersList(Bookstore bookStore)
        {
            var workersList = new List<Worker>();
            var queryString = "SELECT * FROM STAFF";
            var command = new NpgsqlCommand(queryString, _sqlconnection.CreateConnection.Connection);
            var reader = command.ExecuteReader();
            var changeManager = new WorkerChangeManager(this);
            var workerFactory = new WorkerFactory(changeManager);
            foreach (var entry in reader)
            {
                workersList.Add((Worker)workerFactory?.makeDBWorker(
                                reader["full_name"].ToString(),
                                reader["job_type"].ToString(),
                                reader["salary"].ToString(),
                                reader["shift_id"].ToString(),
                                "unimplemented login",
                                "unimplemented  password",
                                reader["employee_id"].ToString()
                                ));
            }
            return workersList;
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
