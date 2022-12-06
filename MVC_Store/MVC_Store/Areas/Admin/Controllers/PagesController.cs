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

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // обьявляю модель PageVM
            PageVM model;
            using (Db db = new Db())
            {
                //получение страницы
                PagesDTO dto = db.Pages.Find(id);
                //проверка доступности страницы
                if(dto == null)
                {
                    return Content("The page is not exist.");
                }
                //инициализация модели данными
                model = new PageVM(dto);
            }
            //возвращаем модель в представление
            return View(model);
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //проверка модели на валидность
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                //получаем id страницы
                int id = model.Id;
                //обьявляю переменную для Slug
                string slug = "home";
                //получение страницы по id
                PagesDTO dto = db.Pages.Find(id);
                //присваиваем название из полученной модели в DTO
                dto.Title = model.Title;
                //проверка краткого заголовка и присваиваем если необходимо
                if(model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                //проверка slug и title на уникальность
                if(db.Pages.Where(x=> x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title alredy exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug)) 
                {
                    ModelState.AddModelError("", "That slug alredy exist.");
                    return View(model);
                }
                //записываем остальные значения в класс DTO
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                //сохранение изменений в базу
                db.SaveChanges();
            }
            //оповещение пользователя в TempData
            TempData["SM"] = "You have edited the page.";
            //переадресация пользователя
            return RedirectToAction ("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //обьяление модели PageVM
            PageVM model;
            using (Db db = new Db())
            {
                //получение страницы
                PagesDTO dto = db.Pages.Find(id);
                //проверка доступности страницы
                if(dto == null)
                {
                    return Content("This page dors not exist.");
                }
                //присваиваем модели все поля из базы
                model = new PageVM(dto);
            }
            //возвращаем модель в представление
            return View(model);
        }
        
    }

}