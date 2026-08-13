using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BiliBilisim_Proje.Models;
using PagedList;

namespace BiliBilisim_Proje.Controllers
{
    public class urunlersController : Controller
    {
        private bili_Entities db = new bili_Entities();

        // GET: urunlers
        public ActionResult Index(int? sayfa, int? ustId, int? altId, int? filtre)
        {
            List<SelectListItem> filtreListesi = new List<SelectListItem>(){
        new SelectListItem{Text="A --> Z",Value="1"},
        new SelectListItem{Text="Z --> A",Value="2"},
        new SelectListItem{Text="Fiyata göre artan",Value="3"},
        new SelectListItem{Text="Fiyata göre azalan",Value="4"}
    };

            ViewBag.filtre = filtreListesi;
            ViewBag.AnaKategoriler = db.ust_kategori.ToList();
            ViewBag.AltKategoriler = db.kategori.ToList();
            ViewBag.ustId = ustId;
            ViewBag.altId = altId;

            int sayfa_no = sayfa ?? 1;

            if (filtre == null && Session["filtre"] != null)
            {
                filtre = (int)Session["filtre"];
            }
            else if (filtre != null)
            {
                Session["filtre"] = filtre;
            }

            var urunler = db.urunler.AsQueryable();
            if (ustId != null)
            {
                var altKategoriIds = db.kategori.Where(x => x.u_kate_id == ustId).Select(x => x.kate_no).ToList();
                urunler = urunler.Where(x => altKategoriIds.Contains(x.kate_no));
            }
            else if (altId != null)
            {
                urunler = urunler.Where(x => x.kate_no == altId);
            }

            switch (filtre)
            {
                case 1: urunler = urunler.OrderBy(x => x.urun_adi); break;
                case 2: urunler = urunler.OrderByDescending(x => x.urun_adi); break;
                case 3: urunler = urunler.OrderBy(x => x.fiyati); break;
                case 4: urunler = urunler.OrderByDescending(x => x.fiyati); break;
                default: urunler = urunler.OrderBy(x => x.urun_id); break;
            }

            IPagedList<urunler> urunlerimiz = urunler.ToPagedList(sayfa_no, 6);
            return View(urunlerimiz);
        }


        // GET: urunlers/Details/5
        public async Task<ActionResult> Details(int? id,int? kateno)
        {
            if (id == null)
            {
                return RedirectToAction("error", "home");
            }
            urunler urunlers = await db.urunler.FindAsync(id);
            List <urunler> kate_urun = db.urunler.Where(x => kateno == null || x.kategori.u_kate_id == kateno).ToList();
            if (urunlers == null)
            {
                return RedirectToAction("error", "home");
            }
            ViewBag.items = kate_urun;
            ViewBag.Kategoriler = db.kategori.ToList();
            return View(urunlers);
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
