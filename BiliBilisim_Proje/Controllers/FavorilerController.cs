using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiliBilisim_Proje.Models;

namespace BiliBilisim_Proje.Controllers
{
    public class FavorilerController : Controller
    {
        bili_Entities dbo = new bili_Entities();
        public Favoriler favorileri_getir()
        {
            Favoriler favoriler = (Favoriler)Session["favoriler"];
            if (Session["favoriler"] == null)
            {
                favoriler = new Favoriler();
                Session["favoriler"] = favoriler;
            }
            return favoriler;
        }
        public ActionResult favorileri_goster()
        {
            if (Session["uye"] == null)
            {

                return RedirectToAction("error", "home");
            }
            return View(favorileri_getir());
        }
        public ActionResult favorilerden_kaldir(int? id)
        {
            if (Session["uye"] == null)
            {

                return RedirectToAction("error", "home");
            }
            if (id == null) return RedirectToAction("error", "home");
            var silinecek = dbo.urunler.Find(id);
            if (silinecek == null) return RedirectToAction("error", "home");
            favorileri_getir().favori_sil(silinecek);
            return RedirectToAction("favorileri_goster", "favoriler");
        }
        public ActionResult favorilere_ekle(int? id)
        {
            if (Session["uye"] == null)
            {

                return RedirectToAction("error", "home");
            }
            if (id == null) return RedirectToAction("error", "home");

            var eklenecek = dbo.urunler.FirstOrDefault(x => x.urun_id == id);
            if (eklenecek == null) return RedirectToAction("error", "home");

            var favoriler = favorileri_getir();
            var varMi = favoriler.Favorilerim.Any(x => x.urun_id == id);

            bool isAdded;
            string mesaj;

            if (varMi)
            {
                favoriler.favori_sil(eklenecek); 
                isAdded = false;
                mesaj = "Favorilerden çıkarıldı";
            }
            else
            {
                favoriler.favori_ekle(eklenecek);
                isAdded = true;
                mesaj = "Favorilere eklendi";
            }

            return Json(new
            {
                success = true,
                isAdded = isAdded,
                message = mesaj
            }, JsonRequestBehavior.AllowGet);
        }

    }
}