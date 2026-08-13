using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiliBilisim_Proje.Models;
using PagedList;
using PagedList.Mvc;

namespace BiliBilisim_Proje.Controllers
{
    public class AdminController : Controller
    {
        bili_Entities dbo = new bili_Entities();
        public ActionResult AdminGiris()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminGiris(string username,string password)
        {
            var admin = dbo.admin.FirstOrDefault(x => x.kuladi == username && x.sifre == password);
            if ( admin != null)
            {
                Session["admin"] = admin;
                TempData["KayitBasarili"] = "Giriş Başarılı.";
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.Hata = "Kullanıcı Adı Veya Şifre Yanlış";
                return View();
            }
        }
        public ActionResult Index()
        {
            if (Session["admin"] == null) return HttpNotFound();
            return View();
        }
        public ActionResult AdminLogout()
        {
            if (Session["admin"] == null) return HttpNotFound();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("AdminGiris", "Admin");
        }
        //------------------------------------------------Üyeler Bölümü----------------------------------------------------------------------
        public ActionResult UyeListele(int? sayfa)
        {
            if (Session["admin"] == null) return HttpNotFound();
            int sayfa_no = sayfa ?? 1;
            IPagedList<uyeler> uyeler = dbo.uyeler.OrderBy(x=>x.ad_soyad).ToPagedList(sayfa_no, 3);
            return View(uyeler);
        }


        //-----------------------------------------------Kategori Bölümü---------------------------------------------------------------------
        public ActionResult KategoriListele(int? sayfa)
        {
            if (Session["admin"] == null) return HttpNotFound();
            int sayfa_no = sayfa ?? 1;
            var kategori = dbo.kategori.GroupBy(x => x.ust_kategori.u_kate_adi).OrderBy(x => x.Key);
            return View(kategori.ToPagedList(sayfa_no, 1));
        }
        //----------------------------------------------Siparişler Bölümü--------------------------------------------------------------------
        public ActionResult SiparisListele(int? sayfa)
        {
            if (Session["admin"] == null) return HttpNotFound();
            int sayfa_no = sayfa ?? 1;
            var siparisler = dbo.siparisler.GroupBy(x => x.uyeler.kuladi).OrderBy(x => x.Key);
            return View(siparisler.ToPagedList(sayfa_no, 1));
        }
        //----------------------------------------------Ürünler Bölümü-----------------------------------------------------------------------
        public ActionResult UrunListele(int? sayfa)
        {
            if (Session["admin"] == null) return HttpNotFound();
            int sayfa_no = sayfa ?? 1;
            var urunler = dbo.urunler.GroupBy(x => x.kategori.kate_adi).OrderBy(x => x.Key).ToPagedList(sayfa_no, 1);
            return View(urunler);
        }
    }
}