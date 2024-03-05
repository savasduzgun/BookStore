using BookStore.Entities;
using BookStore.Panel.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security.Policy;

namespace BookStore.Panel.Controllers
{
    public class CategoriesController : Controller
    {


        SqlConnection connection = new SqlConnection();
        public CategoriesController(IConfiguration configuration)
        {

            connection.ConnectionString = configuration.GetConnectionString("BookStoreContext");
        }


        public IActionResult Index()
        {
            //Bu kodları SqlHelper sınıfında metot olarak tanımladık

            //SqlDataAdapter adapter = new SqlDataAdapter("select * from dbo.Categories order by Name", connection);
            //DataTable dt = new DataTable();
            //adapter.Fill(dt);

            List<Category> list = new List<Category>();
            var rows = SqlHelper.GetRows("select * from dbo.Categories order by Name", connection, null);
            foreach (DataRow row in rows)
            {
                list.Add(new Category
                {
                    Id = Convert.ToInt32(row["Id"].ToString()),        //gelen değerleri convert edip listenin içine eklerim
                    Name = row["Name"].ToString()
                });
            }

            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                //Name primary key olduğu için gelen kategori adı kayıtlı mı değil mi diye bakmam gerekiyor.
                //Kayıtlı ise geriye hata dönmesi , kayıtlı değilse de kayıt işlemini yapması gerekiyor.
                SqlCommand cmd = new SqlCommand("insert into dbo.Categories values (@name)", connection);
                cmd.Parameters.AddWithValue("name", model.Name);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction(nameof(Index));
            }
            else
                return View(model);
        }

        public IActionResult Edit(int id)
        {
            List<CommandParameter> parameters = new List<CommandParameter>();
            parameters.Add(new CommandParameter { Name = "id", Value = id });
            var rows = SqlHelper.GetRows("select * from dbo.Categories where Id=@id", connection, parameters);

            if (rows != null && rows.Count > 0)
            {
                Category category = new Category();
                category.Id = Convert.ToInt32(rows[0]["Id"]);
                category.Name = rows[0]["Name"].ToString();

                return View(category);
            }
            else
                return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                SqlDataAdapter da = new SqlDataAdapter("select count(*) from dbo.Categories where Id=@id", connection);
                da.SelectCommand.Parameters.AddWithValue("id", model.Id);
                DataTable dt = new DataTable();
                da.Fill(dt);
                int kayitSayisi = Convert.ToInt32(dt.Rows[0][0]);  //datatable içerisindeki row ların içindeki ilk row un ilk hücresinin değeri
                if (kayitSayisi == 0)
                {
                    ModelState.AddModelError(string.Empty, model.Id + " numaralı kayıt bulunamadı"); //edit.cshtml ModelOnly de hata olarak en üstte görünmesi için string.Empty gönderdik
                    return View(model);
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("update dbo.Categories set Name=@name where Id=@id", connection);
                    cmd.Parameters.AddWithValue("id", model.Id); //model içindeki Id yi alıp geriye göndersin 
                    cmd.Parameters.AddWithValue("name", model.Name);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
                return View(model);
        }

        public ActionResult Delete(int id)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Categories where Id=@id", connection); //veri çekmek için SqlDataAdapter. bütün kayıtları değil ilgili id li kayıt için where clause
            da.SelectCommand.Parameters.AddWithValue("id", id); //id parametresini dolduralım ki bu değer sql tarafına gitsin
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)  // veri tabanından id kayıtı çektik bu kayıt var mı diye bakıyoruz
            {
                return RedirectToAction(nameof(Index));  // veritabanında bu kaydı bulamadıysak direkt index ekranına gönderdik
            }
            else
            {
                Category category = new Category();  //veritabanında kayıt bulduysak view i açtırmamız gerekir ama category tipinde model göndermemiz lazım o yüzden nesne oluşturduk
                category.Id = Convert.ToInt32(dt.Rows[0]["Id"]);  //datatable içindeki row ların içindeki ilk row u yani indexi 0 olan row un Id kolonunun değerini al int e çevir nesnenin Id sine yaz
                category.Name = dt.Rows[0]["Name"].ToString();

                return View(category);
            }
        }

        [HttpPost]
        public IActionResult Delete(Category model)  // içine id değil category nesnesi alacak
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Categories where Id=@id", connection);
            da.SelectCommand.Parameters.AddWithValue("id", model.Id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0) //ilgili kayıt var mı yok mu diye bakılır 
            {
                ModelState.AddModelError(string.Empty, model.Id + "numaralı kategori bulunamadı. "); //eğer bir kayıt yoksa geriye hata döndürmemiz gerekiyor. key herkangi bir property e bağlı olmadığı için string.Empty diyerek
                                                                                                     //ekranın en üstünde hata yazar  
                return View(model);  //delete ekranını açtırırız gelen modeli tekrar gönderiyoruz
            }
            else    // Kayıt varsa
            {
                SqlCommand cmd = new SqlCommand("delete from dbo.Categories where Id=@id", connection); // nesneyi oluşturup içerisine delete kodumuzu yazarak ilgili delete işlemini yapmasını sağlarız
                cmd.Parameters.AddWithValue("id", model.Id);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
