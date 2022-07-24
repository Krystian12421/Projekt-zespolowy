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
using ProjektZespolowy;
namespace Projekt_zespolowy
{
    /// <summary>
    /// Logika interakcji dla klasy AddBook.xaml
    /// </summary>
    public partial class AddBook : Window
    {
        SqlConnect con = new SqlConnect();
        private int UserId;
        private bool Admin;
        public AddBook(int _UserId, bool _Admin)
        {
            InitializeComponent();
            UserId = _UserId;
            Admin = _Admin;
        }

        private void bt_submit_Click(object sender, RoutedEventArgs e)
        {
            string title = Input_Title.Text;
            string author = Input_Autor.Text;
            string ISBN = Input_ISBN.Text;
            string url = Input_PDF.Text;
            string tag = Input_TAG.Text;
            double x = 1, y = 1, z = 1;
            /*
            
            

            using (Py.GIL())
            {
                from gensim.models.doc2vec import Doc2Vec, TaggedDocument
                from nltk.tokenize import word_tokenize

                model = Doc2Vec(vector_size=3,
                alpha=0.025, 
                min_alpha=0.00025,
                min_count=1,
                dm =1)
                model.load("d2v.model")

                bookstr = author + ' ' + tag

                book_tokenized = word_tokenize(bookstr.lower())
                book_vector = model.infer_vector(book_tokenized)
                x = book_vector[0]
                y = book_vector[1]
                z = book_vector[2]
                
                Console.ReadKey();
            }
            */
            try
            {
                using (SqlConnection connection = con.Connection())
                {
                    string query = string.Format(@"IF NOT EXISTS (SELECT ISBN FROM [dbo].books WHERE ISBN={0})
                                            BEGIN
                                                INSERT INTO [dbo].books(Title, Author, ISBN, LINK, X, Y, Z) VALUES('{1}','{2}','{3}','{4}', {5}, {6}, {7});
                                                INSERT INTO [dbo].tagbook(BookId, TagId) SELECT BookId, TagId FROM [dbo].books, [dbo].tags 
                                                WHERE [dbo].books.ISBN = {8} AND [dbo].tags.Tag = {9};
                                            END;", ISBN, title, author, ISBN, url, x.ToString(), y.ToString(), z.ToString(), ISBN, tag);

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                            MessageBox.Show("Pomyślnie dodałeś nową książkę!");
                        else
                            MessageBox.Show("Nie udało się dodać nowej książki!");
                    }
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
            ShowBooks window = new ShowBooks(1, true);
            window.Show();
            this.Close();
        }
    }
}
