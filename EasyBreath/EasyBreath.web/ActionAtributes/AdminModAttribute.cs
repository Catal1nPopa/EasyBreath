﻿using EasyBreath.BussinessLogic.Interfaces;
using EasyBreath.Domain.Enum;
using EasyBreath.web.Extensions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EasyBreath.web.ActionAtributes
{
     public class AdminModAttribute : ActionFilterAttribute
     {
          private readonly ISession _sessionBusinessLogic;

          public AdminModAttribute()
          {
               var businessLogic = new BussinessLogic.BusinessLogic();
               _sessionBusinessLogic = businessLogic.GetSessionBL();
          }

          public override void OnActionExecuting(ActionExecutingContext filterContext)
          {
               var apiCookie = HttpContext.Current.Request.Cookies["X-KEY"];
               if (apiCookie != null)
               {
                    var profile = _sessionBusinessLogic.GetUserByCookie(apiCookie.Value);
                    if (profile != null && profile.AccessLevel == URole.ADMINISTRATOR)
                    {
                         HttpContext.Current.SetMySessionObject(profile);
                    }
                    else
                    {
                         filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                         {
                              controller = "Home",
                              action = "Index"
                         }));
                    }
               }
               else
               {

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                         controller = "Home",
                         action = "Index"
                    }));
               }
          }
     }
}