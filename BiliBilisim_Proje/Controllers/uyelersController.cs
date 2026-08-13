using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using BiliBilisim_Proje.Models;

namespace BiliBilisim_Proje.Controllers
{
    public class uyelersController : Controller
    {
        private bili_Entities db = new bili_Entities();


        public async Task<ActionResult> Edit()
        {
            if (Session["uye"] == null)
            {
                return RedirectToAction("error", "home");
            }

            var oturumdakiUye = (uyeler)Session["uye"];
            uyeler uyeler = await db.uyeler.FindAsync(oturumdakiUye.uye_id);

            if (uyeler == null)
            {
                return RedirectToAction("error", "home");
            }

            ViewBag.plaka = new SelectList(db.sehirler, "plaka", "il", uyeler.plaka);
            return View(uyeler);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "uye_id,kuladi,sifre,ad_soyad,dog_tar,cinsiyet,vip,adres,email,plaka,vip_basvuru")] uyeler uyeler, string confirm)
        {
            if (Session["uye"] == null)
            {
                return RedirectToAction("error", "home");
            }

            var oturumdakiUye = (uyeler)Session["uye"];

            if (uyeler.sifre != confirm)
            {
                ModelState.AddModelError("confirm", "Şifreler birbiriyle uyuşmuyor!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var guncellenecek_uye = await db.uyeler.FindAsync(oturumdakiUye.uye_id);

                    if (guncellenecek_uye != null)
                    {
                        bool yenivip = (guncellenecek_uye.vip_basvuru == false && uyeler.vip_basvuru == true);

                        guncellenecek_uye.sifre = uyeler.sifre;
                        guncellenecek_uye.cinsiyet = uyeler.cinsiyet;
                        guncellenecek_uye.plaka = uyeler.plaka;
                        guncellenecek_uye.ad_soyad = uyeler.ad_soyad;
                        guncellenecek_uye.dog_tar = uyeler.dog_tar;
                        guncellenecek_uye.adres = uyeler.adres;
                        guncellenecek_uye.vip_basvuru = uyeler.vip_basvuru;
                        guncellenecek_uye.email = uyeler.email;

                        db.Entry(guncellenecek_uye).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        TempData["KayitBasarili"] = "Bilgileriniz başarıyla güncellenmiştir.";

                        if (yenivip)
                        {
                            await BayilikMailGonderAsync(
                                guncellenecek_uye.email,
                                guncellenecek_uye.ad_soyad,
                                guncellenecek_uye.adres,
                                guncellenecek_uye.kuladi,
                                guncellenecek_uye.sehirler.il
                            );
                        }

                        Session["uye"] = guncellenecek_uye;

                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    string msj = ex.GetBaseException().Message;
                    if (msj.Contains("uyeler_email_key")) ModelState.AddModelError("KayitHata", "Bu email zaten mevcut");
                    else if (msj.Contains("uyeler_kuladi_key")) ModelState.AddModelError("KayitHata", "Bu kullanıcı adı zaten mevcut");
                    else ModelState.AddModelError("", "Hata oluştu: " + msj);
                }
            }

            ViewBag.plaka = new SelectList(db.sehirler, "plaka", "il", uyeler.plaka);
            return View(uyeler);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (Session["uye"] == null)
            {

                return RedirectToAction("error", "home");
            }
            if (id == null)
            {
                return RedirectToAction("error", "home");
            }
            uyeler uyeler = await db.uyeler.FindAsync(id);
            if (uyeler == null)
            {
                return RedirectToAction("error", "home");
            }
            db.uyeler.Remove(uyeler);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        private async Task BayilikMailGonderAsync(string kullaniciEmail, string adSoyad, string adres,string kuladi,string sehir)
        {
            try
            {

                string gondericiMail = "bilibilisim92@gmail.com";
                string sifre = "lfhc gmgf rcub jbnh";
                string aliciMail = "bilibilisimadmn@outlook.com"; // Mailin kime gideceği 

                // 2. Mail İçeriği
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(gondericiMail, "Bilişim Proje Sistemi");
                mail.To.Add(aliciMail);

                

                mail.Subject = "Yeni Bayilik Talebi Alındı!";
                mail.IsBodyHtml = true;
                mail.Body = $@"
            <div style='font-family:Arial; padding:20px; border:1px solid #ddd;'>
                <h2 style='color:#2c3e50;'>Yeni Bayilik Başvurusu</h2>
                <p>Sistem üzerinden bir kullanıcı bayilik talebinde bulunuldu. Detaylar aşağıdadır:</p>
                <hr/>
                <p><strong>Ad Soyad:</strong> {adSoyad}</p>
                <p><strong>Kullanıcı Adı:</strong> {kuladi}</p>
                <p><strong>Email Adresi:</strong> {kullaniciEmail}</p>
                <p><strong>İl:</strong> {sehir}</p>
                <p><strong>Adres:</strong> {adres}</p>
                <br/>
                <p><small>Bu mail sistem tarafından otomatik olarak gönderilmiştir.</small></p>
            </div>";

                // 3. SMTP Sunucu Ayarları 
                // Eğer Yandex kullanıyorsanız: smtp.yandex.com.tr (Port: 465 veya 587)
                // Eğer Gmail ise: "smtp.office365.com" (Port: 587)
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587); 
                smtp.Credentials = new NetworkCredential(gondericiMail, sifre); 
                smtp.EnableSsl = true;

                // 4. Maili Gönder
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        [HttpGet]
        public ActionResult sifre_unut()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> sifre_unut(string kul_veri, string sifre,string sifreon)
        {
            var degis_uye = await db.uyeler.FirstOrDefaultAsync(x => x.kuladi == kul_veri || x.email == kul_veri);
            if (sifre.Length<8)
            {
                ViewBag.sifrehata = "Şifre en az 8 karakter olmalı!";
            }
            else
            {
            if (sifre != sifreon)
            {
                        ViewBag.sifreuyus = "Şifreler Birbiri ile uyuşmuyor";
            }
            else if (degis_uye != null)
            {
                degis_uye.sifre = sifre;

                db.Entry(degis_uye).State = EntityState.Modified;
                await db.SaveChangesAsync();

                ViewBag.basari = "Şifreniz başarıyla güncellendi!";
            }
            else
            {
                ViewBag.kulhata = "Kullanıcı adı veya e-mail bulunamadı";
            }
         }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
