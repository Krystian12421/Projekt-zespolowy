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
using System.Data;
using System.Text.RegularExpressions;
using ProjektZespolowy;

namespace Projekt_zespolowy
{
    /// <summary>
    /// Logika interakcji dla klasy FavouriteBooks.xaml
    /// </summary>
    public partial class FavouriteBooks : Window
    {
        SqlConnect con = new SqlConnect();
        private int UserId;
        private bool Admin;
        public FavouriteBooks(int _UserId, bool _Admin)
        {
            InitializeComponent();
            UserId = _UserId;
            Admin = _Admin;
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
        /// <summary>
        /// Metoda zapełniajaca DataTable wykorzystując kwerendę SQL.
        /// </summary>
        private void books_sql()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = con.Connection())
                {
                    connection.Open();

                    string sql = string.Format(String.Format(@"SELECT [dbo].books.BookId
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
                                                        INNER JOIN [dbo].favourites
                                                          ON [dbo].books.BookId = [dbo].favourites.BookId WHERE [dbo].favourites.UserId = {0}
                                                        GROUP BY [dbo].books.Title,[dbo].authors.Author,[dbo].books.ISBN, [dbo].books.BookId;",UserId));

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
        /// <summary>
        /// Metoda służąca do wylogowania się 
        /// </summary>
        private void bt_logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        }
        /// <summary>
        /// Metoda zamykająca aplikację
        /// </summary>
        private void bt_close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Metoda służąca do powrotu do listy wszystkich książek
        /// </summary>
        private void bt_show_books_Click(object sender, RoutedEventArgs e)
        {
            ShowBooks window = new ShowBooks(1, true);
            window.Show();
            this.Close();
        }
    }
}
