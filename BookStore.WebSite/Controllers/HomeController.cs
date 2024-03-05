using BookStore.Entities;
using BookStore.WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BookStore.WebSite.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection connection = new SqlConnection();
        public HomeController(IConfiguration configuration)
        {
            connection.ConnectionString = configuration.GetConnectionString("BookStoreContext");
        }

        public IActionResult Index()
        {
            ViewBag.Categories = GetCategories();  //hangi view açılırsa açılsın hepsine erişmek için kullandım çünkü bu viewden değil layouttan erişeceğim için o yüzden index view e göndermedim aynı zamanda indexten de erişebilirim
            HomePageViewModel model = new HomePageViewModel();
            model.SizinIcinSectiklerimiz = GetBooks(false);
            model.CokSatanlar = GetBooks(true);
            model.secilenler = GetSlide(true);

            return View(model);//model olarak buraya gönderirsem sadece index ten erişebilirdim
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

        private List<Book> GetBooks(bool isBestSeller) //kitapları çekmek için method oluşturdum
        {
            string sqlCommand = "select top 4 * from dbo.Books where IsSelected=1 order by Name";
            if (isBestSeller) //metoda true gönderdiğimizde bu kodu çalıştıracak
            {
                sqlCommand = "select top 4 * from dbo.Books where IsBestSeller=1 order by Name";
            }
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand, connection);
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
                    WriterId = Convert.ToInt32(row["WriterId"]),
                    IsSelected = Convert.ToBoolean(row["IsSelected"]),
                    IsBestSeller = Convert.ToBoolean(row["IsBestSeller"])
                };
                books.Add(book);
            }
            return books;
        }

        private List<Slide> GetSlide(bool isActive)
        {
            string sqlCommand = "select top 3 * from dbo.Slides where IsActive=0 order by OrderNo ";
            if (isActive)
            {
                sqlCommand = "select top 3 * from dbo.Slides where IsActive=1 order by OrderNo";
            }
            SqlDataAdapter da = new SqlDataAdapter(sqlCommand, connection);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<Slide> slides = new List<Slide>();

            foreach (DataRow row in dt.Rows)
            {
                Slide slide = new Slide
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    OrderNo = Convert.ToInt32(row["OrderNo"]),
                    SlideUrl = row["SlideUrl"].ToString(),
                    Url = row["Url"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                };
                slides.Add(slide);
            }
            return slides;
        }
    }
}
