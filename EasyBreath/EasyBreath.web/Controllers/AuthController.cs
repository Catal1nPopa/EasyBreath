﻿using EasyBreath.BussinessLogic.Interfaces;
using EasyBreath.Domain.Entities.User;
using EasyBreath.web.ActionAtributes;
using EasyBreath.web.Extensions;
using EasyBreath.web.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using static EasyBreath.BussinessLogic.Core.SessionApi;

namespace EasyBreath.web.Controllers
{
     public class AuthController : BaseController
     {
          private readonly ISession _session;

          public AuthController()
          {
               var bl = new BussinessLogic.BusinessLogic();
               _session = bl.GetSessionBL();

          }

          public ActionResult RegisterPage()
          {
               ViewBag.Message = "Register Page";
               return View();
          }

          [HttpPost]
          public ActionResult RegisterPage(RegisterForm data)
          {
               if (ModelState.IsValid)
               {
                    RegisterData uRegister = new RegisterData
                    {
                         Username = data.Username,
                         Password = data.Password,
                         Email = data.Email,
                    };

                    var response = _session.ValidateUserRegister(uRegister);
                    if (response.Status)
                    {
                         return RedirectToAction("LoginPage", "Auth");
                    }
                    else
                    {
                         ModelState.AddModelError("", response.StatusMessage);
                         return View(data);
                    }
               }
               return View();
          }

          public ActionResult LoginPage()
          {
               ViewBag.Message = "Login Page";
               return View();
          }

          [HttpPost]
          public ActionResult LoginPage(LoginForm data)
          {
               if (ModelState.IsValid)
               {
                    LoginData uLogin = new LoginData
                    {
                         Username = data.Username,
                         Password = data.Password,
                         Time = DateTime.Now,
                    };

                    var response = _session.ValidateUserCredential(uLogin);
                    SessionStatus();
                    if (response.Status)
                    {
                         
                         var cookieResponse = _session.GenCookie(data.Username);
                         if (cookieResponse != null)
                         {
                              ControllerContext.HttpContext.Response.Cookies.Add(cookieResponse.Cookie);
                              return RedirectToAction("Index", "Home");
                         }
                         else
                         {
                              throw new Exception();
                         }

                    }
                    else
                    {
                         ViewBag.Error = "Invalid Username or password.";
                         ModelState.AddModelError("Invalid Username or password.", response.StatusMessage);
                         ViewData["LoginFlag"] = "Invalid Username or Password!";
                         return View();
                    }
               }
               return View();
          }

          [AuthorizedMod]
          public ActionResult Logout()
          {
               System.Web.HttpContext.Current.Session.Clear();
               if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("X-KEY"))
               {
                    var cookie = ControllerContext.HttpContext.Request.Cookies["X-KEY"];
                    if (cookie != null)
                    {
                         cookie.Expires = DateTime.Now.AddDays(-1);
                         ControllerContext.HttpContext.Response.Cookies.Add(cookie);
                    }
               }

               System.Web.HttpContext.Current.Session["LoginStatus"] = "logout";

               return RedirectToAction("Index", "Home");
          }

     }
}