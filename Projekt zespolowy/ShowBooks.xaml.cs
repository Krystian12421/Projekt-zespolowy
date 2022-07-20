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
    /// Logika interakcji dla klasy ShowBooks.xaml
    /// </summary>
    public partial class ShowBooks : Window
    {
        mysql_connect con = new mysql_connect();
        private string login;
        public ShowBooks(string _login)
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
                MySqlCommand command1 = new MySqlCommand("SELECT Title,Author,Date,ISBN,Tag FROM books", connection);
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

        private void bt_read(object sender, RoutedEventArgs e)
        {
            var index = dg_data.SelectedIndex;
            if (index != -1)
            {
                DataRowView row = (DataRowView)dg_data.SelectedItems[0];
                var value = row["ISBN"];
                try
                {
                    MySqlConnection connection = new MySqlConnection(con.connect());
                    connection.Open();
                    MySqlCommand command1 = new MySqlCommand("SELECT Link FROM books WHERE ISBN = '" + value + "'", connection);
                    var test = command1.ExecuteScalar().ToString();
                    System.Diagnostics.Process.Start(test);

                    connection.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
        }

        private void bt_like(object sender, RoutedEventArgs e)
        {
            var index = dg_data.SelectedIndex;
            if (index != -1)
            {
                DataRowView row = (DataRowView)dg_data.SelectedItems[0];
                var value = row["ISBN"];
                MySqlConnection connection = new MySqlConnection(con.connect());
                connection.Open();
                string sql_insert = "INSERT INTO favourite (login, ISBN) SELECT" + "'" + login + "','" + value + "'" + " FROM DUAL WHERE NOT EXISTS (SELECT login FROM favourite WHERE login='" + login + "' AND ISBN = '" + value + "')";
                MySqlCommand command = new MySqlCommand(sql_insert, connection);
                try
                {
                    if (command.ExecuteNonQuery() == 1)
                    {
                        MessageBox.Show("Pomyślnie dodałeś ksiazke do swojej biblioteki!");
                    }
                    else
                    {
                        MessageBox.Show("Taka ksiazka istnieje juz w twojej bibliotece!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void bt_show_favourite_Click(object sender, RoutedEventArgs e)
        {
            FavouriteBooks window = new FavouriteBooks(login);
            window.Show();
            this.Close();
        }
    }
}
