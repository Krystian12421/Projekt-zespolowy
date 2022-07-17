using System;
using MySql.Data.MySqlClient;
namespace ClassLibrary1
{
    public class mysql_connect
    {
        private string server;
        private string database;
        private string username;
        private string password;
        private string port;
        public string connectionString;
        public MySqlConnection connection;
        public mysql_connect()
        {
            info();
        }
        /// <summary>
        /// Metaa służąca do zainicjowania danych do połączenia z bazą danych
        /// </summary>
        private void info()
        {
            server = "localhost";
            database = "projekt_zespolowy";
            username = "root";
            password = "";
            port = "3306";
            connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4};", server, port, username, password, database);
        }
        /// <summary>
        /// metoda służąca do zwrócenia danych używanych do połączenia z bazą
        /// </summary>
        /// <returns></returns>
        public string connect()
        {
            return connectionString;
        }
    }
}
