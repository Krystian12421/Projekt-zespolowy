using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using ProjektZespolowy;
namespace Projekt_zespolowy
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnect con = new SqlConnect();
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Metoda służąca do zalogowania użytkownika
        /// </summary>
        private void bt_log_in_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int UserId = 0;
                bool Admin = false;

                using (SqlConnection connection = con.Connection())
                {
                    connection.Open();
                    
                    string sql = string.Format(@"SELECT [dbo].[users].[UserId]
                                ,[dbo].[users].[Admin]
                                FROM[dbo].[users]
                                WHERE[dbo].[users].Login = '{0}'
                                AND [dbo].[users].Haslo = HASHBYTES('SHA2_256', CONVERT(NVARCHAR(255), '{1}'))",
                                input_login.Text, input_password.Text);

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            
                            while (reader.Read())
                            {
                                UserId = reader.GetInt32(0);
                                Admin = reader.GetBoolean(1);
                            }
                            
                        }
                    }
                }
                if (UserId != 0)
                {
                    MessageBox.Show("Zalogowales sie pomyslnie!");
                    ShowBooks window = new ShowBooks(UserId, Admin);
                    window.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Podaj poprawne dane lub załóż konto!");
                }
            }
            catch
            {
                MessageBox.Show("Błąd podczas łączenia z bazą danych");
            }
        }
        /// <summary>
        /// Metoda przenosząca nas do okna odpowiadającego za tworzenie konta
        /// </summary>
        private void bt_register_Click(object sender, RoutedEventArgs e)
        {
            register_window window = new register_window();
            window.Show();
            this.Close();
        }
    }
}
