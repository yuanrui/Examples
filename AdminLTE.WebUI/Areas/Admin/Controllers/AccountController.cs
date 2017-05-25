using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdminLTE.WebUI.Common;
using AdminLTE.WebUI.Models;

namespace AdminLTE.WebUI.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Admin/Account/

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(BaseCriteria criteria)
        {
            Pager<AccountEntity> result = null;
            using (var ctx = SimpleDataContext.Current)
            {
                result = ctx.Set<AccountEntity>().AsQueryable().AsPager(criteria.PageIndex, criteria.PageSize);
            }

            return Json(result.AsDataGrid(), JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Admin/Account/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Admin/Account/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Account/Create

        [HttpPost]
        public ActionResult Create(AccountEntity ent)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/Account/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Admin/Account/Edit/5

        [HttpPost]
        public ActionResult Edit(AccountEntity ent)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Delete(IList<AccountEntity> list)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
