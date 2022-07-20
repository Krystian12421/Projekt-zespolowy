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
using ClassLibrary1;
namespace Projekt_zespolowy
{
    /// <summary>
    /// Logika interakcji dla klasy AddBook.xaml
    /// </summary>
    public partial class AddBook : Window
    {
        mysql_connect con = new mysql_connect();
        private string login;
        public AddBook(string _login)
        {
            InitializeComponent();
            login = _login;
        }

        private void bt_submit_Click(object sender, RoutedEventArgs e)
        {
                MySqlConnection connection = new MySqlConnection(con.connect());
                connection.Open();
                    string sql_insert = "INSERT INTO books(Title,Author,Date,ISBN, LINK,Tag) VALUES('" + (Input_Title.Text) + "','" + Input_Autor.Text + "','" + input_Date.Text + "','" + Input_ISBN.Text + "','" + Input_PDF.Text + "','" + Input_TAG.Text + "')";
                    MySqlCommand command = new MySqlCommand(sql_insert, connection);
                    try
                    {
                        if (command.ExecuteNonQuery() == 1)
                        {
                            MessageBox.Show("Pomyślnie dodałeś nową książkę!");
                        }
                        else
                        {
                            MessageBox.Show("Nie udało się dodać nowej książki!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
        }

        private void bt_logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void bt_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bt_delete_Click(object sender, RoutedEventArgs e)
        {
            ShowBooks window = new ShowBooks(login);
            window.Show();
            this.Close();
        }
    }
}
