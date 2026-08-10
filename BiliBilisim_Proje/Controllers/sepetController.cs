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
            ViewBag.msj = msj;
            return View(sepeti_getir());
        }
        public ActionResult sepete_ekle(int? id,int? adet)
        {
            if (id == null) return HttpNotFound();
            if (adet < 0) return HttpNotFound();
            var _adet = adet ?? 0;
            var sepete_eklenecek = db.urunler.FirstOrDefault(x => x.urunid == id);
            sepeti_getir().sepet_ekle(sepete_eklenecek, _adet);
            return RedirectToAction("sepeti_goster");
        }
        public ActionResult sepetten_sil(int? id)
        {
            if (id == null) return HttpNotFound();
            var silinecek = db.urunler.Find(id);
            sepeti_getir().sepetten_sil(silinecek);
            return RedirectToAction("sepeti_goster");
        }
        public ActionResult sepeti_temizle()
        {
            
            sepeti_getir().sepetten_temizle();
            return RedirectToAction("sepeti_goster");
        }
    }
}