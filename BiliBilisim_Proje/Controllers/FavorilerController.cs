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
           
            return View(favorileri_getir());
        }
        public ActionResult favorilerden_kaldir(int? id)
        {
            if (id == null) return HttpNotFound();
            var silinecek = dbo.urunler.Find(id);
            if (silinecek == null) return HttpNotFound();
            favorileri_getir().favori_sil(silinecek);
            return RedirectToAction("favorileri_goster", "favoriler");
        }
        public ActionResult favorilere_ekle(int? id)
        {
            if (id == null) return HttpNotFound();
            var eklenecek = dbo.urunler.FirstOrDefault(x => x.urun_id == id);
            favorileri_getir().favori_ekle(eklenecek);
            return RedirectToAction("favorileri_goster","favoriler");
        }

    }
}