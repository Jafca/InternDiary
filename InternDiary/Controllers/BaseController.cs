﻿using InternDiary.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace InternDiary.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext db = new ApplicationDbContext();
        protected string _userId;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _userId = User.Identity.GetUserId();
            base.OnActionExecuting(filterContext);
        }
    }
}