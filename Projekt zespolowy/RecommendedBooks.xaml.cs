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
    /// Interaction logic for RecommendedBooks.xaml
    /// </summary>
    public partial class RecommendedBooks : Window
    {
        SqlConnect con = new SqlConnect();
        private int UserId;
        private bool Admin;
        public RecommendedBooks(int _UserId, bool _Admin)
        {
            InitializeComponent();
            UserId = _UserId;
            Admin = _Admin;
            grid_books();
            books_sql();
        }
        private void grid_books()
        {
            dg_data.Columns.Add(new DataGridTextColumn() { Header = "Podobieństwo", Binding = new Binding("Similarity") });
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
                                                        ,[dbo].books.X
                                                        ,[dbo].books.Y
                                                        ,[dbo].books.Z
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
                                                        WHERE [dbo].books.BookId NOT IN
                                                                                 (SELECT [dbo].books.BookId FROM [dbo].books
                                                                                  JOIN [dbo].favourites
                                                                                    ON [dbo].books.BookId = [dbo].favourites.BookId
                                                                                  JOIN [dbo].users
                                                                                    ON [dbo].favourites.UserId = [dbo].users.UserId)
                                                        GROUP BY [dbo].books.Title,[dbo].authors.Author,[dbo].books.X,[dbo].books.Y,
                                                        [dbo].books.Z,[dbo].books.ISBN, [dbo].books.BookId;");

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                dt = CosineSimilarity(dt);
                fillgrid(dt);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private DataTable CosineSimilarity(DataTable dt)
        {
            double avgX = 0, avgY = 0, avgZ = 0;
            try
            {
                using (SqlConnection connection = con.Connection())
                {
                    connection.Open();

                    string sql = string.Format(@"SELECT Avg(X) AS AVGX, Avg(Y) AS AVGY, Avg(Z) AS AVGY FROM [dbo].books 
                                                JOIN [dbo].favourites
                                                  ON [dbo].books.BookId = [dbo].favourites.BookId
                                                JOIN [dbo].users
                                                  ON [dbo].favourites.UserId = [dbo].users.UserId");

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                avgX = reader.GetDouble(0);
                                avgY = reader.GetDouble(1);
                                avgZ = reader.GetDouble(2);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            dt.Columns.Add("Similarity", typeof(double));

            double magnitudeAvg = Math.Sqrt(Math.Pow(avgX, 2) + Math.Pow(avgY, 2) + Math.Pow(avgZ, 2));
            double dotProduct, magnitude, similarity;
            double x, y, z;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                x = Convert.ToDouble(dt.Rows[i]["X"]);
                y = Convert.ToDouble(dt.Rows[i]["Y"]);
                z = Convert.ToDouble(dt.Rows[i]["Z"]);
                dotProduct = (avgX * x) + (avgY * y) + (avgZ * z);
                magnitude = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
                similarity = dotProduct / magnitude * magnitudeAvg;
                dt.Rows[i]["Similarity"] = similarity;
            }
            return dt;
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
        private void bt_show_books_Click(object sender, RoutedEventArgs e)
        {
            ShowBooks window = new ShowBooks(1, true);
            window.Show();
            this.Close();
        }
    }
}
