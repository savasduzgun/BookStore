using BookStore.Entities;
using BookStore.Panel.Helpers;
using BookStore.Panel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Intrinsics.Arm;

namespace BookStore.Panel.Controllers
{
    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        SqlConnection connection = new SqlConnection();
        public BooksController(IConfiguration configuration, IWebHostEnvironment environment)
        {

            connection.ConnectionString = configuration.GetConnectionString("BookStoreContext");
            _environment = environment;
        }
        public IActionResult Index()
        {
            SqlDataAdapter da = new SqlDataAdapter("select b.*, c.Name as CategoryName, w.NameSurname as WriterName from dbo.Books as b inner join dbo.Categories as c on c.Id=b.CategoryId inner join dbo.Writers as w on w.Id=b.WriterId order by b.Name", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<BookViewModel> books = new List<BookViewModel>();

            foreach (DataRow row in dt.Rows)    
            {
                BookViewModel book = new BookViewModel
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
                    CategoryName = Convert.ToString(row["CategoryName"]),
                    WriterName = Convert.ToString(row["WriterName"]),
                    IsSelected = Convert.ToBoolean(row["IsSelected"]),
                    IsBestSeller = Convert.ToBoolean(row["IsBestSeller"])
                };
                books.Add(book);
            }
            return View(books);
        }

        public IActionResult Create() // Yeni bir book create edileceği için, elde bir veri yok parametre almaz
        {
            //ViewBag.KategoriListesi = GetCategories(); BookCrateModel ile yaptım ihtiyaç kalmadı 

            BookCreateModel createmodel = new BookCreateModel
            {
                Book = new Book (),
                Categories = GetCategories(), 
                Writers = GetWriters()
            };
            return View(createmodel);
        }

        [HttpPost]

        public IActionResult Create(BookCreateModel model, IFormFile file)
        {
            if (ModelState.IsValid) 
            {
                //resim.png
                string wwwwRootPath = _environment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                string newFileName = fileName + "-" + DateTime.Now.ToString("yyyyMMddhhmmfff") + extension;        //resim-20240212173400123.png 

                string path = Path.Combine(wwwwRootPath + "/uploads/books/", newFileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }


                SqlCommand cmd = new SqlCommand("insert into dbo.Books values (@name, @categoryId, @writerId, @pageCount, @price, @publishDate, @description, @imageUrl, '', '', @isSelected, @isBestSeller)", connection);
                cmd.Parameters.AddWithValue("name", model.Book.Name);
                cmd.Parameters.AddWithValue("categoryId", model.Book.CategoryId);
                cmd.Parameters.AddWithValue("writerId", model.Book.WriterId);
                cmd.Parameters.AddWithValue("pageCount", model.Book.PageCount);
                cmd.Parameters.AddWithValue("price", model.Book.Price);
                cmd.Parameters.AddWithValue("publishDate", model.Book.PublishDate);
                cmd.Parameters.AddWithValue("description", model.Book.Description);
                cmd.Parameters.AddWithValue("imageUrl", newFileName);
                cmd.Parameters.AddWithValue("isSelected", model.Book.IsSelected);
                cmd.Parameters.AddWithValue("isBestSeller", model.Book.IsBestSeller);


                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                //ViewBag.KategoriListesi = GetCategories();
                BookCreateModel createmodel = new BookCreateModel
                {
                    Book = model.Book,
                    Categories = GetCategories(),
                    Writers = GetWriters()
                };
                return View(createmodel);
                //return View(model);
            }
                
        } 

        public List<Category> GetCategories() 
        {
            List<Category> list = new List<Category>();

            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Categories order by Name", connection); 
            DataTable dt = new DataTable();  
            da.Fill(dt); 
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Category
                {
                    Id = Convert.ToInt32(row["Id"].ToString()),
                    Name = row["Name"].ToString()
                });
            }
            return list;
        }

        public List<Writer> GetWriters()
        {
            List<Writer> list = new List<Writer>();

            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Writers order by NameSurname", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Writer
                {
                    Id = Convert.ToInt32(row["Id"].ToString()),
                    NameSurname = row["NameSurname"].ToString()
                });
            }
            return list;
        }

        public Book GetBook(int id) //ilgili kitabı çekmek için int id değerini almam gerekiyor ki veritabanında ilgili book kaydını alıp geriye sınıfa convert ederek döndürebiliyim
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Books where Id=@id", connection);
            da.SelectCommand.Parameters.AddWithValue("id", id);
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
                WriterId = Convert.ToInt32(dt.Rows[0]["WriterId"]),
                IsSelected = Convert.ToBoolean(dt.Rows[0]["IsSelected"]),
                IsBestSeller = Convert.ToBoolean(dt.Rows[0]["IsBestSeller"])
            };

            return book;
        }

        public BookViewModel GetBookViewModel(int id) 
        {
            SqlDataAdapter da = new SqlDataAdapter("select b.*, c.Name as CategoryName, w.NameSurname as WriterName from dbo.Books as b inner join dbo.Categories as c on c.Id=b.CategoryId inner join dbo.Writers as w on w.Id=b.WriterId where b.Id=@id", connection);
            da.SelectCommand.Parameters.AddWithValue("id", id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            BookViewModel book = new BookViewModel
            {
                Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                CategoryId = Convert.ToInt32(dt.Rows[0]["CategoryId"]),
                Description = Convert.ToString(dt.Rows[0]["Description"]),
                ImageUrl = dt.Rows[0]["ImageUrl"].ToString(),
                Name = dt.Rows[0]["Name"].ToString(),
                PageCount = Convert.ToInt32(dt.Rows[0]["PageCount"]),
                Price = Convert.ToDouble(dt.Rows[0]["Price"]),
                PublishDate = Convert.ToDateTime(dt.Rows[0]["PublishDate"]),
                WriterId = Convert.ToInt32(dt.Rows[0]["WriterId"]),
                CategoryName= dt.Rows[0]["CategoryName"].ToString(),
                WriterName= dt.Rows[0]["writerName"].ToString(),
                IsSelected = Convert.ToBoolean(dt.Rows[0]["IsSelected"]),
                IsBestSeller = Convert.ToBoolean(dt.Rows[0]["IsBestSeller"])
            };

            return book;
        }
        public IActionResult Edit(int id) //Edit ekranını açtırırken view e içinde yazarlar ve kategoriler olan iki tane liste gönderiyoruz
        {
            ViewBag.KategoriListesi = GetCategories();
            ViewBag.YazarListesi = GetWriters();
            //Book book = GetBook(id);
            //return View(book);
            return View(GetBook(id));
        }

        [HttpPost]

        public IActionResult Edit(Book model, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if(file== null) //view den aldığımız dosya geldi mi gelmedi mi kontrol
                {
                    model.ImageUrl = GetBook(model.Id).ImageUrl; //veritabanından ilgili book u bulup ImageUrl değerini aynı şekilde tekrar modele yaz aynı kalsın diye
                }
                else
                {
                    //resim.png
                    string wwwwRootPath = _environment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string newFileName = fileName + "-" + DateTime.Now.ToString("yyyyMMddhhmmfff") + extension;        //resim-20240212173400123.png 
                    model.ImageUrl = newFileName; //veritabanındaki değeri buraya yazmamız gerekiyor

                    string path = Path.Combine(wwwwRootPath + "/uploads/books/", newFileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
               
                SqlCommand cmd = new SqlCommand("update dbo.Books set Name=@name, CategoryId=@categoryId, WriterId=@writerId, PageCount=@pageCount, Price=@price, PublishDate=@publishDate, Description=@description, ImageUrl=@imageUrl, IsSelected=@isSelected, IsBestSeller=@isBestSeller where Id=@id", connection);
                cmd.Parameters.AddWithValue("id", model.Id);
                cmd.Parameters.AddWithValue("name", model.Name);
                cmd.Parameters.AddWithValue("categoryId", model.CategoryId);
                cmd.Parameters.AddWithValue("writerId", model.WriterId);
                cmd.Parameters.AddWithValue("pageCount", model.PageCount);
                cmd.Parameters.AddWithValue("price", model.Price);
                cmd.Parameters.AddWithValue("publishDate", model.PublishDate);
                cmd.Parameters.AddWithValue("description", model.Description);
                cmd.Parameters.AddWithValue("imageUrl", model.ImageUrl);
                cmd.Parameters.AddWithValue("isSelected", model.IsSelected);
                cmd.Parameters.AddWithValue("isBestSeller", model.IsBestSeller);
             

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.KategoriListesi = GetCategories();
                ViewBag.YazarListesi = GetWriters();
              
                return View(model);
            }

        }

        public IActionResult Delete(int id) //Delete ekranında silmek istediğimiz hangi kitabın bilgilerini göstermek istiyorsam id parametresi alarak ilgili kaydı veri tababnında bulup view e gönderebilirim 
        {

            return View(GetBookViewModel(id));
        }
        [HttpPost]
        public IActionResult Delete (Book book) 
        {
            BookViewModel book1 = GetBookViewModel(book.Id); //id ye istek attık değer veri tabanında var mı diye kontrol ediyorum
            if (book1 == null) 
            {
                ModelState.AddModelError(string.Empty, book.Id + " numaralı kayıt sistemde bulunamadı.");
                return View(book1);
            }
            else 
            {
                string wwwRootPath = _environment.WebRootPath;
                string path = Path.Combine(wwwRootPath + "/uploads/books/", book1.ImageUrl);  //C:\Github\savasduzgun\BookStore\BookStore.Panel\wwwroot\uploads\books/aklindanbirsayitut-202402131141479.jpg 

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                SqlCommand cmd = new SqlCommand("delete from dbo.Books where Id = @id", connection);
                cmd.Parameters.AddWithValue("id", book.Id);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                return RedirectToAction(nameof(Index));  //kitapları listelediğimiz index view ine gitmek için
            }
        }
    }
}
