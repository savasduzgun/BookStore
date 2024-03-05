using BookStore.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BookStore.Panel.Controllers
{
    public class WritersController : Controller
    {
        SqlConnection connection = new SqlConnection();
        public WritersController(IConfiguration configuration)
        {

            connection.ConnectionString = configuration.GetConnectionString("BookStoreContext");
        }
        public IActionResult Index()
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Writers order by NameSurname", connection);  //database den verileri çekerim
            DataTable dt = new DataTable();   //Herhangi bir parametremiz olmadığı için AddWithValue değil datatable kullanırım
            da.Fill(dt);   // dataadapter ın fill metoduyla dt datatable nesnesinin içerisine tüm bulduğu kayıtları doldururum

            //Şimdi yapmamız gereken view e bir liste göndermemiz gerektiği için  datatable da bulduğumuz verileri bir listenin  biriktirmek 

            List<Writer> writers = new List<Writer>();  //Writer tipinde bir liste oluşturuyoruz içi boş bir şekilde writers nesnesini oluşturdum. İçini databaseden alıp verileri yazdığım dt nesnesinin içinden yazarım.

            foreach (DataRow row in dt.Rows)           //dt nesnesinin içinde rowların içinde tek tek dönerek row ve kolonların değerlerini alıp Writer sınıfından birer nesne oluşturarak bu listenin içine eklerim
            {
                Writer writer = new Writer
                {
                    Id = Convert.ToInt32(row["Id"]),
                    NameSurname = row["NameSurname"].ToString()
                };
                writers.Add(writer);  //oluşturduğumuz bu writer nesnesini listenin içine eklerim
            }
            return View(writers);   //bu verileri yani writers nesnesini index e model olarak gönderirim
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Writer model)
        {
            if (ModelState.IsValid)
            {
                SqlCommand cmd = new SqlCommand("insert into dbo.Writers values (@name)", connection);
                cmd.Parameters.AddWithValue("name", model.NameSurname); //name parametresinin değerine modelden gelen NameSurname verisini veririm ki değeri modelin içinde alıp veri tabanına yazsın.

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                return RedirectToAction(nameof(Index)); //kaydetme işleminden sonra index e gitsin.
            }
            else
                return View(model);
        }

        public IActionResult Edit(int id)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Writers Where Id=@id", connection); //ilgili id ye sahip kayıt var mı ?
            da.SelectCommand.Parameters.AddWithValue("id", id);   //parametre eklediğimiz için ilgili parametrenin değerini doldururuz. parametre olarak gelen id yi buraya value olarak veriririm

            DataTable dt = new DataTable();
            da.Fill(dt);          // data table in içine veritabanından gelen tüm satırları yazarım.

            if (dt.Rows.Count == 0)  //gelen data table ın içinde herhangi bir row var mı yok mu
                return RedirectToAction(nameof(Index));  //Eğer kayıt yoksa index e git yani listeye yönlendiririz.
            else
            {
                Writer writer = new Writer
                {
                    Id = Convert.ToInt32(dt.Rows[0]["Id"]), //id alanı için dt nin ilk satırındaki Id kolonundaki veriyi al Id propertysine at. burdan bize object döner Id ise int.    
                    NameSurname = dt.Rows[0]["NameSurname"].ToString()
                };
                //artık elimizde bir writer nesnem var ve bilgileri de veri tabanındakilerle dolu olacak şekilde buraya geldi

                return View(writer); //bulmuş olduğum bu nesnesi göndererek view in açılmasını sağlarım
            }
        }

        [HttpPost]
        public IActionResult Edit(Writer model)
        {
            if (ModelState.IsValid) 
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Writers Where Id=@id", connection); //ilgili id ye sahip kayıt var mı ?
                da.SelectCommand.Parameters.AddWithValue("id", model.Id);   //parametre eklediğimiz için ilgili parametrenin değerini doldururuz. parametre olarak gelen id yi buraya value olarak veriririm

                DataTable dt = new DataTable();
                da.Fill(dt);          // data table in içine veritabanından gelen tüm satırları yazarım.

                if (dt.Rows.Count == 0)  //gelen data table ın içinde herhangi bir row var mı yok mu
                {
                    ModelState.AddModelError(string.Empty, model.Id + " numaralı kayıt bulunamadı.");  //kayıt yoksa ekrana hata fırlatırız.
                    return View(model);
                }
                else     //kayıt varsa
                {
                    SqlCommand cmd = new SqlCommand("update dbo.Writers set NameSurname=@name where Id=@id", connection);  //NameSurname alanına name parametresinde yazan değeri gönder
                    cmd.Parameters.AddWithValue("id", model.Id); //model içindeki Id yi alıp geriye göndersin 
                    cmd.Parameters.AddWithValue("name", model.NameSurname);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                return View(model);  //model valid değilse girilen değerler uygun değilse
            }
        }

        public IActionResult Delete(int id)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Writers Where Id=@id", connection); //ilgili id ye sahip kayıt var mı ?
            da.SelectCommand.Parameters.AddWithValue("id", id);   //parametre eklediğimiz için ilgili parametrenin değerini doldururuz. parametre olarak gelen id yi buraya value olarak veriririm

            DataTable dt = new DataTable();
            da.Fill(dt);          // data table in içine veritabanından gelen tüm satırları yazarım.

            if (dt.Rows.Count == 0)  //gelen data table ın içinde herhangi bir row var mı yok mu
                return RedirectToAction(nameof(Index));  //Eğer kayıt yoksa index e git yani listeye yönlendiririz.
            else
            {
                Writer writer = new Writer
                {
                    Id = Convert.ToInt32(dt.Rows[0]["Id"]), //id alanı için dt nin ilk satırındaki Id kolonundaki veriyi al Id propertysine at. burdan bize object döner Id ise int.    
                    NameSurname = dt.Rows[0]["NameSurname"].ToString()
                };
                //artık elimizde bir writer nesnem var ve bilgileri de veri tabanındakilerle dolu olacak şekilde buraya geldi

                return View(writer); //bulmuş olduğum bu nesnesi göndererek view in açılmasını sağlarım
            }
        }

        [HttpPost]
        public IActionResult Delete(Writer model)
        {       //Burada modelstate bakmaya gerek yok çünkü sadece id geliyor olacak adı vesaire gelmesine gerek yok

                SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Writers Where Id=@id", connection); //veritabanında ilgili id ye sahip kayıt var mı ?
                da.SelectCommand.Parameters.AddWithValue("id", model.Id);   //parametre eklediğimiz için ilgili parametrenin değerini doldururuz. parametre olarak gelen id yi buraya value olarak veririm

                DataTable dt = new DataTable();
                da.Fill(dt);          // data table in içine veritabanından gelen tüm satırları yazarım.

                if (dt.Rows.Count == 0)  //gelen data table ın içinde herhangi bir row var mı yok mu
                {
                    ModelState.AddModelError(string.Empty, model.Id + " numaralı kayıt bulunamadı.");  //kayıt yoksa ekrana hata fırlatırız.
                    return View(model);
                }
                else     //kayıt varsa
                {
                    SqlCommand cmd = new SqlCommand("delete from dbo.Writers where Id=@id", connection);  //yanlış kaydı silmemek için where clause kullandım
                    cmd.Parameters.AddWithValue("id", model.Id); //model içindeki Id yi alıp geriye göndersin 

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    return RedirectToAction(nameof(Index));
                }
            }
        }
    }
