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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using ClassLibrary1;

namespace Projekt_zespolowy
{
    /// <summary>
    /// Logika interakcji dla klasy register_window.xaml
    /// </summary>
    public partial class register_window : Window
    {
        mysql_connect con = new mysql_connect();
        public register_window()
        {
            InitializeComponent();
        }
        private void button_register_Click(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(input_register_login.Text, @"^\p{N}||\p{L}{1,20}$") &&
                Regex.IsMatch(input_register_password.Text, @"^\p{N}||\p{L}{1,20}$"))
            {
                if (input_register_password.Text == input_register_password2.Text)
                {
                    try
                    {
                        MySqlConnection connection = new MySqlConnection(con.connect());
                        connection.Open();
                        MySqlCommand command1 = new MySqlCommand("SELECT Login FROM uzytkownicy WHERE Login = '" + input_register_login.Text + "'", connection);
                        if (command1.ExecuteScalar() == null)
                        {

                            string sql_insert = "INSERT INTO uzytkownicy(Login,Haslo) VALUES('" + (input_register_login.Text) + "','" + input_register_password.Text + "')";
                            MySqlCommand command = new MySqlCommand(sql_insert, connection);
                            try
                            {
                                if (command.ExecuteNonQuery() == 1)
                                {
                                    MessageBox.Show("Konto zostało pomyślnie utworzone!");
                                }
                                else
                                {
                                    MessageBox.Show("Nie udało się utworzyć konta!");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Takie konto już istnieje!");
                        }
                        connection.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Błąd podczas łączenia z bazą danych");
                    }
                }
                else
                {
                    MessageBox.Show("Hasła muszą być identyczne!");
                }
            }
            else
            {
                MessageBox.Show("Liczba znaków nie może przekraczać 20!");
            }
        }
        /// <summary>
        /// Metoda odpowiadająca za powrót do okna logowania
        /// </summary>
        private void bt_back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }
    }
}
