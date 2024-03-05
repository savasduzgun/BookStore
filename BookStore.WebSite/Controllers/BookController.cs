using BookStore.Entities;
using BookStore.WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BookStore.WebSite.Controllers
{
    public class BookController : Controller
    {
        SqlConnection connection = new SqlConnection();
        public BookController(IConfiguration configuration)
        {
            connection.ConnectionString = configuration.GetConnectionString("BookStoreContext");
        }
        public IActionResult Index(int id)
        {
            ViewBag.Categories = GetCategories();

            BookViewModel viewModel = new BookViewModel();
            viewModel.Book = GetBook(id);
            viewModel.Books = GetBooks();
            viewModel.Category = GetCategory(viewModel.Book.CategoryId);
            viewModel.Writer = GetWriter(viewModel.Book.WriterId);
          
            return View(viewModel);
        }

        private List<Category> GetCategories()
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Categories order by Name", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<Category> categories = new List<Category>(); //Category class ı entites de olduğu için reference edilir ve using satırı olarak eklenir

            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new Category
                {
                    Id = Convert.ToInt32(row["Id"].ToString()),
                    Name = row["Name"].ToString()
                });
            }

            return categories;
        }

        private Category GetCategory(int categoryId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Categories where Id=@id", connection);
            da.SelectCommand.Parameters.AddWithValue("id", categoryId);
            DataTable dt = new DataTable();
            da.Fill(dt);



            Category category = new Category
            {
                Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                Name = dt.Rows[0]["Name"].ToString()
            };


            return category;
        }

        private Writer GetWriter(int writerId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Writers where Id=@id", connection);
            da.SelectCommand.Parameters.AddWithValue("id", writerId);
            DataTable dt = new DataTable();
            da.Fill(dt);



            Writer writer = new Writer
            {
                Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                NameSurname = dt.Rows[0]["NameSurname"].ToString()
            };


            return writer;
        }

        private List<Book> GetBooks()
        {
            SqlDataAdapter da = new SqlDataAdapter("select top 4 * from dbo.Books where IsSelected=1 order by Name", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<Book> books = new List<Book>();

            foreach (DataRow row in dt.Rows)
            {
                Book book = new Book
                {
                    Id = Convert.ToInt32(row["Id"]),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    Description = Convert.ToString(row["Description"]),
                    ImageUrl = row["ImageUrl"].ToString(),
                    Name = row["Name"].ToString(),
                    PageCount = Convert.ToInt32(row["PageCount"]),
                    Price = Convert.ToDouble(row["Price"]),
                    PublishDate = Convert.ToDateTime(row["PublishDate"]),
                    WriterId = Convert.ToInt32(row["WriterId"])
                };
                books.Add(book);
            }
            return books;
        }

        private Book GetBook(int bookId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Books where Id=@id", connection);
            da.SelectCommand.Parameters.AddWithValue("id", bookId);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Book book = new Book
            {
                Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                CategoryId = Convert.ToInt32(dt.Rows[0]["CategoryId"]),
                Description = Convert.ToString(dt.Rows[0]["Description"]),
                ImageUrl = dt.Rows[0]["ImageUrl"].ToString(),
                Name = dt.Rows[0]["Name"].ToString(),
                PageCount = Convert.ToInt32(dt.Rows[0]["PageCount"]),
                Price = Convert.ToDouble(dt.Rows[0]["Price"]),
                PublishDate = Convert.ToDateTime(dt.Rows[0]["PublishDate"]),
                WriterId = Convert.ToInt32(dt.Rows[0]["WriterId"])
            };

            return book;
        }
    }
}
