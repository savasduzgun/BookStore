using BookStore.Entities;
using BookStore.WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BookStore.WebSite.Controllers
{
    public class CategoriesController : Controller
    {
        SqlConnection connection = new SqlConnection();
        public CategoriesController(IConfiguration configuration)
        {
            connection.ConnectionString = configuration.GetConnectionString("BookStoreContext");
        }

        public IActionResult Index(int id)
        {

            ViewBag.Categories = GetCategories();

            CategoryViewModel viewModel = new CategoryViewModel
            {
                Category = GetCategory(id),
                Books = GetBooks(id)
            };

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

        private List<Book> GetBooks(int categoryId)
        {
            SqlDataAdapter da = new SqlDataAdapter("select* from dbo.Books where CategoryId=@catId", connection);
            da.SelectCommand.Parameters.AddWithValue("catId", categoryId);
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
    }
}

