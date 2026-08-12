using BiliBilisim_Proje.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace BiliBilisim_Proje.Controllers
{
    public class HomeController : Controller
    {
        bili_Entities dbo = new bili_Entities();
        public ActionResult Index(int? id)
        {
            IEnumerable<urunler> urunlerimiz = null;

                urunlerimiz = dbo.urunler.Where(x => id == null || x.kate_no == id).ToList();
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
        //[HttpPost]
        //public ActionResult Contact(string customerName, string customerEmail, string contactSubject, string contactMessage)
        //{
        //        try
        //        {
        //            MailMessage mail = new MailMessage();

        //            // 1. Gönderici Hesabı (Yeni Gmail Adresin)
        //            mail.From = new MailAddress("bilibilisim92@gmail.com", "BİLİBİLİŞİM İletişim Formu");

        //            // 2. Alıcı Hesabı (Mesajların düşeceği Outlook Adresin)
        //            mail.To.Add("bilibilisimadmn@outlook.com");

        //            mail.Subject = contactSubject;
        //            mail.IsBodyHtml = true;

        //            mail.Body = $"<h3>Yeni İletişim Formu Mesajı</h3>" +
        //                        $"<b>Gönderen:</b> {customerName} <br/>" +
        //                        $"<b>Email:</b> {customerEmail} <br/><br/>" +
        //                        $"<b>Mesaj:</b> <br/> {contactMessage}";

        //            // Gmail SMTP Sunucu Ayarları
        //            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        //            smtp.UseDefaultCredentials = false;
        //            // Gmail adresin ve 16 haneli Uygulama Şifren
        //            smtp.Credentials = new NetworkCredential("bilibilisim92@gmail.com", "lfhc gmgf rcub jbnh");
        //            smtp.EnableSsl = true;

        //            smtp.Send(mail);

        //            ViewBag.msj = "Mesajınız başarıyla gönderildi!";
        //        }
        //        catch (Exception ex)
        //        {
        //        ViewBag.msj = "Mesaj gönderilirken bir hata oluştu! "+ex.Message;
        //        }

        //    return View();
        //}

        [HttpPost]
        public ActionResult MailGonder(string customerName, string customerEmail, string contactSubject, string contactMessage)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("bilibilisim92@gmail.com", "BİLİBİLİŞİM İletişim Formu");
                mail.To.Add("bilibilisimadmn@outlook.com");
                mail.Subject = contactSubject;
                mail.IsBodyHtml = true;

                mail.Body = $"<h3>Yeni İletişim Formu Mesajı</h3>" +
                            $"<b>Gönderen:</b> {customerName} <br/>" +
                            $"<b>Email:</b> {customerEmail} <br/><br/>" +
                            $"<b>Mesaj:</b> <br/> {contactMessage}";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("bilibilisim92@gmail.com", "lfhc gmgf rcub jbnh");
                smtp.EnableSsl = true;

                smtp.Send(mail);
                
                // Doğrudan JS alert basıp sayfaya yönlendirir
                return Content("<script>alert('Mesajınız başarıyla gönderildi!'); window.location.href='/Home/Contact';</script>");
            }
            catch (Exception ex)
            {
                return Content($"<script>alert('Hata oluştu: {ex.Message}'); window.location.href='/Home/Contact';</script>");
            }
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
                    TempData["KayitBasarili"] = "Giriş Başarılı,Ana Sayfaya Yönlendiriliyorsunuz.";
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
                        TempData["KayitBasarili"] = "Kayıt Başarılı";
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
                if (msj.Contains("uyeler_email_key")) ModelState.AddModelError("GuncellemeHata", "Bu email zaten mevcut");
                else if (msj.Contains("uyeler_kuladi_key")) ModelState.AddModelError("GuncellemeHata", "Bu kullanıcı adı zaten mevcut");
               
            }
            
            ViewBag.plaka = new SelectList(dbo.sehirler, "plaka", "il");
            return View("Kaydol_Giris");
        }
        public ActionResult Logout()
        {
            if (Session["uye"] == null)
            {

                return RedirectToAction("error", "home");
            }
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