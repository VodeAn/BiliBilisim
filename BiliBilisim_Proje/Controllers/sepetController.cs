using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiliBilisim_Proje.Models;
namespace BiliBilisim_Proje.Controllers
{
    public class sepetController : Controller
    {
        // GET: sepet
        bili_Entities db = new bili_Entities();
        public Sepet sepeti_getir()
        {
            Sepet sepet = (Sepet)Session["sepet"];
            if(Session["sepet"] == null)
            {
                sepet = new Sepet();
                Session["sepet"] = sepet;
            }
            return sepet;
        }
        public ActionResult sepeti_goster(string msj)
        {
            if (Session["uye"] == null || !((uyeler)Session["uye"]).vip)
            {

                return RedirectToAction("error", "home");
            }
            ViewBag.msj = msj;
            return View(sepeti_getir());
        }
        [HttpPost]
        public JsonResult AdetGuncelle(int id, int adet)
        {
            if (Session["uye"] != null || ((uyeler)Session["uye"]).vip)
            {
                var urun = db.urunler.Find(id);

                if (urun != null)
                {
                    if (adet >= urun.stok)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Bu üründen en fazla {urun.stok} adet alabilirsiniz.",
                            maksStok = urun.stok 
                        });
                    }
                    var sepet = sepeti_getir();
                    var guncellenecekUrun = sepet.Sepetim.FirstOrDefault(x => x.urun.urun_id == id);

                    if (guncellenecekUrun != null)
                    {
                        guncellenecekUrun.adet = adet;
                        Session["sepet"] = sepet;

                        return Json(new { success = true });
                    }
                }
            }

            return Json(new { success = false, message = "Oturum veya ürün hatası." });
        }

        public ActionResult sepete_ekle(int? id,int? adet)
        {
            if (Session["uye"] == null || !((uyeler)Session["uye"]).vip)
            {

                return RedirectToAction("error", "home");
            }
            if (id == null) return RedirectToAction("error", "home");
            if (adet < 0) return RedirectToAction("error", "home");
            var _adet = adet ?? 0;
            var sepete_eklenecek = db.urunler.FirstOrDefault(x => x.urun_id == id);
            var sepet = sepeti_getir();
            var urunvarmi = sepeti_getir().Sepetim.FirstOrDefault(x => x.urun.urun_id == id);
            int sepettekiMevcutAdet = urunvarmi != null ? urunvarmi.adet : 0;
            int istenenToplamAdet = sepettekiMevcutAdet + _adet;
            if (istenenToplamAdet >= sepete_eklenecek.stok)
            {
                TempData["StokHatasi"] = $"Maalesef stoklarımızda sadece {sepete_eklenecek.stok} adet {sepete_eklenecek.urun_adi} bulunmaktadır.";
                if(istenenToplamAdet == 0) TempData["StokHatasi"] = $"Maalesef stoklarımızda  {sepete_eklenecek.urun_adi} bulunmamaktadır.";

                if (Request.UrlReferrer != null)
                    return Redirect(Request.UrlReferrer.ToString());
                return RedirectToAction("index", "home");
            }
            if ( urunvarmi != null)
            {
                urunvarmi.adet++;
            }
            else
            {
                sepeti_getir().sepet_ekle(sepete_eklenecek, _adet);
            }
           
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
            return RedirectToAction("index","home");
        }
        public ActionResult sepetten_sil(int? id)
        {
            if (Session["uye"] == null || !((uyeler)Session["uye"]).vip)
            {

                return RedirectToAction("error", "home");
            }
            if (id == null) return RedirectToAction("error", "home");
            var silinecek = db.urunler.Find(id);
            sepeti_getir().sepetten_sil(silinecek);
            return RedirectToAction("sepeti_goster");
        }
        public ActionResult sepeti_temizle()
        {
            if (Session["uye"] == null || !((uyeler)Session["uye"]).vip)
            {

                return RedirectToAction("error", "home");
            }
            sepeti_getir().sepetten_temizle();
            return RedirectToAction("sepeti_goster");
        }
    }
}