using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using project_ca_nhan.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using project_ca_nhan.Areas.Admin.Attributes;
using System.IO;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlidesController : Controller
    {
        public MyDbcontext db = new MyDbcontext();
        public IActionResult Index()
        {
            return RedirectToAction("Read");
        }
        public IActionResult Read(int? page)
        {
            int _RecordPerPage = 20;
            int _CurrentPage = page ?? 1;
            List<ItemSlide> _ListRecord = db.Slides.OrderByDescending(item => item.Id).ToList();
            return View("Read", _ListRecord.ToPagedList(_CurrentPage, _RecordPerPage));
        }
        public IActionResult update(int? id)
        {
            int _id = id ?? 0;
            ItemSlide record = db.Slides.Where(item => item.Id == _id).FirstOrDefault();
            return View("CreateUpdate", record);
        }
        [HttpPost]
        public IActionResult Update(IFormCollection fc, int? id)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Title = fc["Title"].ToString().Trim();
            string _SubTitle = fc["SubTitle"].ToString().Trim();
            string _Info = fc["Info"].ToString().Trim();
            string _Link = fc["Link"].ToString().Trim();
            int _id = id ?? 0;
            var record = db.Slides.Where(item => item.Id == _id).FirstOrDefault();
            record.Name = _Name;
            record.Title = _Title;
            record.SubTitle = _SubTitle;
            record.Info = _Info;
            record.Link = _Link;
            //---
            string _FileName = "";
            try
            {
                _FileName = Request.Form.Files[0].FileName;
            }
            catch {; }
            if (!String.IsNullOrEmpty(_FileName))
            {
                if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo));
                }
                var timestap = DateTime.Now.ToFileTime();
                _FileName = timestap + "_" + _FileName;
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Slides", _FileName);
                using (var stream = new FileStream(_Path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                record.Photo = _FileName;
            }
            db.SaveChanges();
            return Redirect("/Admin/Slides");
        }
        public IActionResult Create()
        {
            return View("CreateUpdate");
        }
        [HttpPost]
        public IActionResult Create(IFormCollection fc)
        {
            string _Name = fc["Name"].ToString().Trim();
            string _Title = fc["Title"].ToString().Trim();
            string _SubTitle = fc["SubTitle"].ToString().Trim();
            string _Info = fc["Info"].ToString().Trim();
            string _Link = fc["Link"].ToString().Trim();
            ItemSlide record = new ItemSlide();
            record.Name = _Name;
            record.Title = _Title;
            record.SubTitle = _SubTitle;
            record.Info = _Info;
            record.Link = _Link;
            string _FileName = "";
            try
            {
                _FileName = Request.Form.Files[0].FileName;
            }
            catch {; }
            if (!String.IsNullOrEmpty(_FileName))
            {
                var timestap = DateTime.Now.ToFileTime();
                _FileName = timestap + "_" + _FileName;
                string _Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload/Slides", _FileName);
                using (var stream = new FileStream(_Path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
            }
            record.Photo = _FileName;

            db.Slides.Add(record);
            db.SaveChanges();
            return Redirect("/Admin/Slides");
        }
        public IActionResult Delete(int? id)
        {
            int _id = id ?? 0;
            var record = db.Slides.Where(item => item.Id == _id).FirstOrDefault();
            if (record.Photo != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Upload", "Slides", record.Photo));
            }
            db.Slides.Remove(record);
            db.SaveChanges();
            return Redirect("/Admin/Slides");
        }
    }
}
