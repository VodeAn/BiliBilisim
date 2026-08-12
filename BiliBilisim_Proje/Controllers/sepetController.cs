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
            if (Session["sepet"] != null || ((uyeler)Session["uye"]).vip)
            {
                var sepet = (BiliBilisim_Proje.Models.Sepet)Session["sepet"];
                var guncellenecekUrun = sepet.Sepetim.FirstOrDefault(x => x.urun.urun_id == id);

                if (guncellenecekUrun != null)
                {
                    guncellenecekUrun.adet = adet;
                    Session["sepet"] = sepet;

                    return Json(new { success = true });
                }
                
            }

            return Json(new { success = false });
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
            var urunvarmi = sepeti_getir().Sepetim.FirstOrDefault(x => x.urun == sepete_eklenecek);
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