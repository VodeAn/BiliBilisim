using BiliBilisim_Proje.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BiliBilisim_Proje.Controllers
{
    public class HomeController : Controller
    {
        bili_Entities dbo = new bili_Entities();
        public ActionResult Index(int? sayfa, int? id, int? filtre)
        {
            List<SelectListItem> filitre = new List<SelectListItem>(){
                new SelectListItem{Text="A --> Z",Value="1"},
                new SelectListItem{Text="Z --> A",Value="2"},
                new SelectListItem{Text="Fiyata göre artan",Value="3"},
                new SelectListItem{Text="Fiyata göre azalan",Value="4"}
            };

            ViewBag.filtre = filitre;
            int sayfa_no = sayfa ?? 1;

            IPagedList<urunler> urunlerimiz = null;
            if (filtre == null)
            {
                urunlerimiz = dbo.urunler.Where(x => id == null || x.kate_no == id).ToList().ToPagedList(sayfa_no, 27);
            }
            return View(urunlerimiz);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Kaydol_Giris(string msj)
        {
            TempData["SuccessMessage"] = null;
            ViewBag.msj = msj;
            ViewBag.plaka = new SelectList(dbo.sehirler,"plaka","il");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Giris(string kuladilar,string sifreler)
        {
            if (string.IsNullOrWhiteSpace(kuladilar))
            {
                
                ModelState.AddModelError("kuladilar", "Lütfen kullanıcı adınızı giriniz.");
            }

            if (string.IsNullOrWhiteSpace(sifreler))
            {
                ModelState.AddModelError("sifreler", "Lütfen şifrenizi giriniz.");
            }


            if (ModelState.IsValid)
            {
                string msj = "";
                var uye = await dbo.uyeler.FirstOrDefaultAsync(x => x.kuladi == kuladilar && x.sifre == sifreler);
                if ( uye != null)
                {
                    Session["uye"] = uye;
                    msj = "Giriş Başarılı. Ana sayfaya yönlendiriliyorsunuz.";
                    TempData["SuccessMessage"] = msj;
                    return RedirectToAction("Index", "Home", null);
                }
                else
                {
                    ModelState.AddModelError("GirisHata", "Kullanıcı Adı ya da Şifre Yanlış!");

                }

            }
            ViewBag.plaka = new SelectList(dbo.sehirler, "plaka", "il");
            return View("Kaydol_Giris");


        }
        [HttpPost]
        public async Task<ActionResult> Kaydol(uyeler uyeler, string confirm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(confirm))
                {

                    ModelState.AddModelError("KayitHata", "Lütfen şifre doğrulama kısmını giriniz.");
                }
                
                if (ModelState.IsValid)
                {
                    if(uyeler.sifre == confirm)
                    {
                        dbo.uyeler.Add(uyeler);
                        await dbo.SaveChangesAsync();
                    }
                    else
                    {
                        
                        ModelState.AddModelError("KayitHata", "Şifreler birbiriyle uyuşmuyor!");
                    }
                }

            }
            catch(Exception error)
            {
                string msj = error.GetBaseException().Message;
                if (msj.Contains("uyeler_email_key")) ModelState.AddModelError("KayitHata", "Bu email zaten mevcut");
                else if (msj.Contains("uyeler_kuladi_key")) ModelState.AddModelError("KayitHata", "Bu kullanıcı adı zaten mevcut");
               
            }
            
            ViewBag.plaka = new SelectList(dbo.sehirler, "plaka", "il");
            return View("Kaydol_Giris");
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult error()
        {
            
            return View();
        }


    }
}