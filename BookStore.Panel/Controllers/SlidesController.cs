using BookStore.Entities;
using BookStore.Panel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BookStore.Panel.Controllers
{
    public class SlidesController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        SqlConnection connection = new SqlConnection();
        public SlidesController(IConfiguration configuration, IWebHostEnvironment environment)
        {

            connection.ConnectionString = configuration.GetConnectionString("BookStoreContext");
            _environment = environment;
        }
        public IActionResult Index()
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Slides order by OrderNo", connection);
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
            return View(slides);
        }

        public IActionResult Create() 
        { 
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slide model, IFormFile file)
        {
            if (ModelState.IsValid) 
            {
                string wwwwRootPath = _environment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                string newFileName = fileName + "-" + DateTime.Now.ToString("yyyyMMddhhmmfff") + extension;

                string path = Path.Combine(wwwwRootPath + "/uploads/slides/", newFileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                SqlCommand cmd = new SqlCommand("insert into dbo.Slides values (@name, @orderNo, @slideUrl, @url, @isActive)", connection);
                cmd.Parameters.AddWithValue("name", model.Name);
                cmd.Parameters.AddWithValue("orderNo", model.OrderNo);
                cmd.Parameters.AddWithValue("slideUrl", newFileName);
                cmd.Parameters.AddWithValue("url", model.Url);
                cmd.Parameters.AddWithValue("isActive", model.IsActive);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                return RedirectToAction(nameof(Index));
            }
            else
                return View(model);
        }

        public Slide GetSlide(int id)
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from dbo.Slides where Id=@id", connection);
            da.SelectCommand.Parameters.AddWithValue("id", id);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Slide slide = new Slide
            {
                Id = Convert.ToInt32(dt.Rows[0]["Id"]),
                Name = dt.Rows[0]["Name"].ToString(),
                OrderNo = Convert.ToInt32(dt.Rows[0]["OrderNo"]),
                SlideUrl = dt.Rows[0]["SlideUrl"].ToString(),
                Url = dt.Rows[0]["Url"].ToString(),
                IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"])
            };

            return slide;
        }

        public IActionResult Edit(int id)
        {
            return View(GetSlide(id));
        }

        [HttpPost]
        public IActionResult Edit(Slide model, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file==null)
                {
                    model.SlideUrl = GetSlide(model.Id).SlideUrl;
                }
                else
                {
                    string wwwwRootPath = _environment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string newFileName = fileName + "-" + DateTime.Now.ToString("yyyyMMddhhmmfff") + extension;
                    model.SlideUrl = newFileName;
                    string path = Path.Combine(wwwwRootPath + "/uploads/slides/", newFileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
               

                SqlCommand cmd = new SqlCommand("update dbo.Slides set Name=@name, OrderNo=@orderNo, SlideUrl=@slideUrl, Url=@url, IsActive=@isActive where Id=@id", connection);
                cmd.Parameters.AddWithValue("id", model.Id);
                cmd.Parameters.AddWithValue("name", model.Name);
                cmd.Parameters.AddWithValue("orderNo", model.OrderNo);
                cmd.Parameters.AddWithValue("slideUrl", model.SlideUrl);
                cmd.Parameters.AddWithValue("url", model.Url);
                cmd.Parameters.AddWithValue("isActive", model.IsActive);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                return RedirectToAction(nameof(Index));
            }
            else
                return View(model);
        }

        public IActionResult Delete(int id)
        {
            return View(GetSlide(id));
        }

        [HttpPost]
        public IActionResult Delete(Slide slide)
        { 
            Slide slide1 = GetSlide(slide.Id);

            if (slide1 == null) 
            {
                ModelState.AddModelError(string.Empty, slide.Id + " numaralı kayıt sistemde bulunamadı.");
                return View(slide1);
            }
            else
            {
                string wwwRootPath = _environment.WebRootPath;
                string path = Path.Combine(wwwRootPath + "/uploads/slides/", slide1.SlideUrl);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                SqlCommand cmd = new SqlCommand("delete from dbo.Slides where Id = @id", connection);
                cmd.Parameters.AddWithValue("id", slide.Id);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
