using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
//using TinyURLService.Models;


namespace TinyURLService.Controllers
{
    public class HomeController : Controller
    {
        //UrlContext db = new UrlContext();

        TinyURLServiceEntities db = new TinyURLServiceEntities();
        
        public ActionResult Index()
        {   
            db.Configuration.ProxyCreationEnabled = false;
            IEnumerable<URL> model = new List<URL>();
            model = db.URLs.AsEnumerable();           
            return View(model);
        }
        public ActionResult RedirectToLong(string shortURL)
        {
            db.Configuration.ProxyCreationEnabled = false;
            if (string.IsNullOrEmpty(shortURL))
                return RedirectToAction("NotFound", "Home");
            else
            {
                URL url = db.URLs.Where(u => u.ShortUrl == shortURL).FirstOrDefault();

                if (url == null)
                    return RedirectToAction("NotFound", "Home");
                else
                {
                    #region Statistics collected for this URL
                    TinyURLService.Models.UrlStats stats = new TinyURLService.Models.UrlStats(Request);

                    stats.UrlId = url.UrlId; // relation

                    TinyURLService.UrlStat us = new UrlStat();
                    us.UrlId = stats.UrlId;
                    try
                    {
                        db.UrlStats.Add(us);
                        db.SaveChanges();
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                    #endregion

                    url.Hits++; // increment visits
                    db.SaveChanges();
                    Response.StatusCode = 302;
                    return Redirect(url.LongUrl); // redirects to the long URL
                }
            }
        }

        public ActionResult NotFound()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ShorterURL(string longUrl)
        {
            db.Configuration.ProxyCreationEnabled = false;
            if (string.IsNullOrEmpty(longUrl))
                return Json(new { status = false, message = "Please provide URL" }, JsonRequestBehavior.AllowGet);
            else
            {
                if (!new TinyURLService.Models.URLS().HasHTTPProtocol(longUrl))
                    longUrl = "http://" + longUrl;

                // Check if long URL already exists in the database
                URL existingURL = db.URLs.Where(u => u.LongUrl.ToLower() == longUrl.ToLower()).FirstOrDefault();

                if (existingURL == null)
                {
                    TinyURLService.Models.URLS shortUrl = new TinyURLService.Models.URLS()
                    {
                        LongUrl = longUrl,
                        GeneratedDate = DateTime.UtcNow,
                        Hits = 0
                    };

                    TinyURLService.URL sUrl = new TinyURLService.URL()
                    {
                        LongUrl = longUrl,
                        GeneratedDate = DateTime.UtcNow,
                        Hits = 0
                    };

                    string userId = User.Identity.Name;

                    if (shortUrl.CheckLongUrlExists())
                    {
                        shortUrl.GenerateRandomShortUrl();
                        if (!string.IsNullOrEmpty(userId)) // only if user is authorized
                            shortUrl.UserId = userId;

                        sUrl.ShortUrl = shortUrl.ShortUrl;

                        db.URLs.Add(sUrl);
                        try
                        {
                            db.SaveChanges();
                            shortUrl.ShortUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/" + shortUrl.ShortUrl;

                            return Json(new { status = true, url = shortUrl }, JsonRequestBehavior.AllowGet);
                        }
                        catch (DbEntityValidationException exc)
                        {                            
                            return Json(new { status = false, message = exc.Message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                        return Json(new { status = false, message = "Not valid URL provided" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    existingURL.ShortUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/" + existingURL.ShortUrl;       
                    return Json(new { status = true, url = existingURL }, JsonRequestBehavior.AllowGet);
                }
            }
        }
       
    }
}
