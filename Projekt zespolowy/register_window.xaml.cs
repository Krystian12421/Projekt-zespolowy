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
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using ProjektZespolowy;

namespace Projekt_zespolowy
{
    /// <summary>
    /// Logika interakcji dla klasy register_window.xaml
    /// </summary>
    public partial class register_window : Window
    {
        SqlConnect con = new SqlConnect();
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
                    string password = input_register_password2.Text;
                    string login = input_register_login.Text;
                    try
                    {
                        using (SqlConnection connection = con.Connection())
                        {
                            string query = string.Format(@"IF NOT EXISTS (SELECT Login FROM [dbo].users WHERE Login='{0}')
                                            BEGIN
                                                INSERT INTO [dbo].users (Login, Haslo, Admin) VALUES ('{1}', HASHBYTES('SHA2_256', CONVERT(NVARCHAR(255),'{2}')), 0);
                                            END;", login, login, password);

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                connection.Open();
                                int result = command.ExecuteNonQuery();

                                // Check Error
                                if (result < 0)
                                    MessageBox.Show("Konto zostało pomyślnie utworzone!");
                                else
                                    MessageBox.Show("Nie udało się utworzyć konta!");
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
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
