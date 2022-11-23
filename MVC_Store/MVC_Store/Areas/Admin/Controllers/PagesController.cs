using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //обьявляем список для представления PageVM
            List<PageVM> pageList;

            //инициализируем список Db
            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //возвращаем список представления
            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
           //проверка модели на валидность
           if(!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {

                //обьявляем переменную для краткого описания (slug)

                string slug;
                
                //инициализируем класс PageDTO
                PagesDTO dto = new PagesDTO();

                //присваиваем заголовок модели
                dto.Title = model.Title.ToUpper();

                //проверяем, есть ли краткое описание, если нет, присваиваем его
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                
                //проверка уникальности заголовка и краткого описания
                if(db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exist.");
                    return View(model);
                }
                else if (db.Pages.Any(x=>x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model);
                }

                // присваиваем оставшиеся значения модели
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //сохранение модели в базу данных
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            //передаем сообщение через TempData
            TempData["SM"] = "You have added a new page!";

            //переадресовываем пользователя на метод INDEX
            return RedirectToAction("Index");

        }
    }   

}