using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restik_masterserver
{
    internal class DBClient
    {
        private readonly string login = "restik_masterserver";
        private readonly string password = "1234567890";
        public static User? user { get; private set; }
        private static volatile DBClient? _instance;
        private static readonly object _lock = new object();

        private bool isConnected;
        private SqlConnection? sqlConnection;
        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; }
        }
        public SqlConnection? SqlConnection
        {
            get { return sqlConnection; }
            set { sqlConnection = value; }
        }
        public static DBClient GetInstance()
        {
            if (_instance != null)
                return _instance;
            lock (_lock)
                if (_instance == null)
                    _instance = new DBClient();
            return _instance;
        }
        public DBClient()
        {
            Task task = Task.Factory.StartNew(() => new VerifyLicense()); // Проверка наличия лицензии на продукт
        }
        public void Connect(string address, string port) // Подключаемся в базе данных
        {
            try
            {
                string connectionString = $"Server={address}\\SQLEXPRESS,{port};Persist Security Info=False;MultipleActiveResultSets=true;User ID={login};Password={password};Integrated Security=False;TrustServerCertificate=True;";
                SqlConnection = new SqlConnection(connectionString);
                SqlConnection.Open();
                IsConnected = true;
                Console.WriteLine("{0}: OK - Connected to {1}.", DateTime.Now, sqlConnection.Database);
            }
            catch (SqlException ex)
            {
                Console.WriteLine("{0}: ERROR - Connect to {1}.", DateTime.Now, sqlConnection.Database);
                Console.WriteLine(ex.Message);
                IsConnected = false;
            }
        }
        public void NonQuery(string _query) // Метод для запросов к БД с консоли
        {
            if (IsConnected)
            {
                try
                {
                    SqlCommand command = new SqlCommand(_query, sqlConnection);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    Console.WriteLine("{0}: OK - Request was successful.", DateTime.Now);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}: ERROR - Request error.", DateTime.Now);
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("There is no connection to the server!");
            }
        }
        public SqlDataReader Query(string _query) // Метод для запросов к БД с консоли
        {
            if (IsConnected)
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(_query, sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) // если есть данные
                                return reader;
                            else
                                return null;
                        }
                    }
                    Console.WriteLine("{0}: OK - Request was successful.", DateTime.Now);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}: ERROR - Request error.", DateTime.Now);
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            else
            {
                Console.WriteLine("There is no connection to the server!");
                return null;
            }
        }
        public byte[] Login(string username, string hash) // Проверяем есть ли такой пользователь, и отправляем информацию о пользователе в виде байтов
        {
            string sqlExpression = $"SELECT * FROM Users WHERE Username='{username}' AND Password='{hash}'";
            try
            {
                using (SqlCommand command = new SqlCommand(sqlExpression, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read()) // построчно считываем данные
                            {
                                user = new User
                                {
                                    Id = (int)reader["ID"],
                                    Username = (string)reader["Username"],
                                    FullName = (string)reader["Fullname"],
                                    ExpirationLicense = (DateTime)reader["ExpirationLicense"],
                                    LicenseActive = (int)reader["LicenseActive"] == 1 ? true : false
                                };
                            }
                            Console.WriteLine("{0}: OK - User {1} has logged in.", DateTime.Now, user.Username);
                            return user.GetBytes();
                        }
                        else
                        {
                            Console.WriteLine("{0}: ERROR - No data found.", DateTime.Now);
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}: ERROR - Authorization error.", DateTime.Now);
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
