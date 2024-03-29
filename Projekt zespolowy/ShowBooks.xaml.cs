﻿using System;
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
using System.Data;
using System.Text.RegularExpressions;
using ProjektZespolowy;
namespace Projekt_zespolowy
{
    /// <summary>
    /// Logika interakcji dla klasy ShowBooks.xaml
    /// </summary>
    public partial class ShowBooks : Window
    {
        SqlConnect con = new SqlConnect();
        private int UserId;
        private bool Admin;
        public ShowBooks(int _UserId, bool _Admin)
        {
            InitializeComponent();
            UserId = _UserId;
            Admin = _Admin;
            if (Admin == false)
            {
                bt_add_book.Visibility = Visibility.Collapsed;
            }
            grid_books();
            books_sql();
        }
        private void grid_books()
        {
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "Id", Binding = new Binding("BookId") });
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "Tytuł", Binding = new Binding("Title") });
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "Autor", Binding = new Binding("Author") });
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "ISBN", Binding = new Binding("ISBN") });
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "Kategorie", Binding = new Binding("Tags") });
            dg_data.AutoGenerateColumns = false;
        }
        private void fillgrid(DataTable dt)
        {
            dg_data.ItemsSource = dt.DefaultView;
        }
        private void books_sql()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = con.Connection())
                {
                    connection.Open();

                    string sql = string.Format(@"SELECT [dbo].books.BookId
                                                        ,[dbo].authors.Author
                                                        ,[dbo].books.Title
                                                        ,[dbo].books.ISBN
                                                        ,STRING_AGG([dbo].tags.Tag, ', ') AS Tags
                                                        FROM [dbo].books
                                                        JOIN [dbo].tagbook
                                                          ON [dbo].tagbook.BookId = [dbo].books.BookId
                                                        JOIN [dbo].authorbook
                                                          ON [dbo].books.BookId = [dbo].authorbook.BookId
                                                        JOIN [dbo].authors
                                                          ON [dbo].authorbook.AuthorId = dbo.authors.AuthorId
                                                        JOIN [dbo].tags
                                                          ON [dbo].tags.TagId = [dbo].tagbook.TagId
                                                        GROUP BY [dbo].books.Title,[dbo].authors.Author,[dbo].books.ISBN, [dbo].books.BookId;");

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                fillgrid(dt);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void bt_add_book_Click(object sender, RoutedEventArgs e)
        {
            AddBook window = new AddBook(UserId, Admin);
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
            string URL = "";
            if (index != -1)
            {
                DataRowView row = (DataRowView)dg_data.SelectedItems[0];
                var value = row["BookId"];
                try
                {
                    using (SqlConnection connection = con.Connection())
                    {
                        connection.Open();

                        string sql = string.Format("SELECT [dbo].books.URL FROM [dbo].books WHERE [dbo].books.BookId = {0}", value);

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    URL = reader.GetString(0);
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                var sInfo = new System.Diagnostics.ProcessStartInfo(URL)
                {
                    UseShellExecute = true,
                };
                System.Diagnostics.Process.Start(sInfo);
            }
        }

        private void bt_like(object sender, RoutedEventArgs e)
        {
            var index = dg_data.SelectedIndex;
            if (index != -1)
            {
                DataRowView row = (DataRowView)dg_data.SelectedItems[0];
                var value = row["BookId"];
                try
                {
                    using (SqlConnection connection = con.Connection())
                    {
                        string query = string.Format(@"IF NOT EXISTS (SELECT BookId, UserId FROM [dbo].favourites WHERE BookId={0} AND UserId = {1})
                                            BEGIN
                                                INSERT INTO [dbo].favourites (BookId, UserId) VALUES ({2},{3});
                                            END;", value, UserId, value, UserId);

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            connection.Open();
                            int result = command.ExecuteNonQuery();

                            // Check Error
                            if (result < 0)
                                MessageBox.Show("Książka już w ulubionych!");
                            else
                                MessageBox.Show("Dodano do ulubionych.");
                        }
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
            FavouriteBooks window = new FavouriteBooks(UserId, Admin);
            window.Show();
            this.Close();
        }

        private void bt_logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }

        private void bt_show_recommended_Click(object sender, RoutedEventArgs e)
        {
            RecommendedBooks window = new RecommendedBooks(UserId, Admin);
            window.Show();
            this.Close();
        }
    }
}
