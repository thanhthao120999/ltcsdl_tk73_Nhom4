using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using uStora.Model.Models;
using uStora.Service;
using uStora.Web.Models;

namespace uStora.Web.Controllers
{
    public class AdminController : Controller
    {
        private INotificationService _notificationService;
        public AdminController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetNotificationUsers()
        {
            var notificationRegisterTime = Session["UserTime"] != null ? Convert.ToDateTime(Session["UserTime"]) : DateTime.Now;
            var userUnViewedList = _notificationService.GetUnViewedUsers(notificationRegisterTime);
            var userAll = _notificationService.GetAllUsers();
            var userListVm = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<ApplicationUserViewModel>>(userUnViewedList);
           
            if (userUnViewedList.Count() > 0)
            {
                var userUnViewedListVm = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<ApplicationUserViewModel>>(userUnViewedList);
                Session["UserTime"] = DateTime.Now;
                return Json(new
                {
                    data = userUnViewedListVm,
                    status = false
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var userAllVm = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<ApplicationUserViewModel>>(userAll);
                Session["UserTime"] = DateTime.Now;
                return Json(new
                {
                    data = userAllVm,
                    status = true
                }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}