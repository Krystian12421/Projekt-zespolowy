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
using System.Data;
using System.Text.RegularExpressions;
using ClassLibrary1;

namespace Projekt_zespolowy
{
    /// <summary>
    /// Logika interakcji dla klasy FavouriteBooks.xaml
    /// </summary>
    public partial class FavouriteBooks : Window
    {
        mysql_connect con = new mysql_connect();
        private string login;
        public FavouriteBooks(string _login)
        {
            InitializeComponent();
            login = _login;
            grid_books();
            books_sql();
        }
        private void grid_books()
        {
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "Tytuł", Binding = new Binding("Title") });
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "Autor", Binding = new Binding("Author") });
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "Rok wydania", Binding = new Binding("Date") });
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "ISBN", Binding = new Binding("ISBN") });
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "Kategorie", Binding = new Binding("Tag") });
            dg_data.AutoGenerateColumns = false;
        }
        private void fillgrid(MySqlCommand command1)
        {
            MySqlDataAdapter adp = new MySqlDataAdapter(command1);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            dg_data.ItemsSource = dt.DefaultView;
        }
        private void books_sql()
        {
            try
            {
                MySqlConnection connection = new MySqlConnection(con.connect());
                connection.Open();
                var sql_cmd = "SELECT books.Title, books.Author, books.Date, books.ISBN, books.Tag FROM books INNER JOIN favourite ON books.ISBN = favourite.ISBN WHERE favourite.login = '" + login + "'";

                MySqlCommand command1 = new MySqlCommand(sql_cmd, connection);
                fillgrid(command1);

                connection.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void bt_logout_Click(object sender, RoutedEventArgs e)
        {
            AddBook window = new AddBook(login);
            window.Show();
            this.Close();
        }

        private void bt_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bt_show_books_Click(object sender, RoutedEventArgs e)
        {
            ShowBooks window = new ShowBooks(login);
            window.Show();
            this.Close();
        }
    }
}
