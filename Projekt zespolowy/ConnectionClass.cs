using System;
using System.Data.SqlClient;
namespace ProjektZespolowy
{
    public class SqlConnect
    {
        private SqlConnectionStringBuilder builder;
        /// <summary>
        /// Konstruktor SqlConnect z danymi do połączenia z bazą danych.
        /// </summary>
        /// <returns></returns>
        public SqlConnect()
        {
            builder = new SqlConnectionStringBuilder();
            builder.ConnectionString = "Server=tcp:w61895.database.windows.net,1433;Initial Catalog=Projekt_zespolowy;Persist Security Info=False;User ID=adminPZ;Password=PZ123!@#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        /// <summary>
        /// Metoda zwracająca nowe połączenie z bazą.
        /// </summary>
        /// <returns></returns>
        public SqlConnection Connection()
        {
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            return connection;
        }
    }
}
