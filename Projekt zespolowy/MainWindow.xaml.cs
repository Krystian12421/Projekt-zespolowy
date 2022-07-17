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
using MySql.Data.MySqlClient;
using ClassLibrary1;
namespace Projekt_zespolowy
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        mysql_connect con = new mysql_connect();
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
                MySqlConnection connection = new MySqlConnection(con.connect());
                connection.Open();
                MySqlCommand command1 = new MySqlCommand("SELECT Login FROM uzytkownicy  WHERE Login = '" + input_login.Text + "'", connection);
                MySqlCommand command2 = new MySqlCommand("SELECT Haslo FROM uzytkownicy  WHERE Login = '" + input_login.Text + "'", connection);
                if (!(command1.ExecuteScalar() == null) && !(command2.ExecuteScalar() == null))
                {
                    MessageBox.Show("Zalogowales sie pomyslnie!");
                    ShowBooks window = new ShowBooks(input_login.Text);
                    window.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Podaj poprawne dane lub załóż konto!");
                }
                connection.Close();
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
