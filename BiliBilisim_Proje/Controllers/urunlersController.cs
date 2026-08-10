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
                urunlerimiz = db.urunler.Where(x => id == null || x.kate_no == id).ToList().ToPagedList(sayfa_no,6);
            }
            else if (filtre == 1)
            {
                urunlerimiz = db.urunler.Where(x => id == null || x.kate_no == id).OrderBy(x => x.urun_adi).ToList().ToPagedList(sayfa_no, 6);
                Session["filtre"] = 1;
            }
            else if (filtre == 2)
            {
                urunlerimiz = db.urunler.Where(x => id == null || x.kate_no == id).OrderByDescending(x => x.urun_adi).ToList().ToPagedList(sayfa_no, 6);
                Session["filtre"] = 2;
            }
            else if (filtre == 3)
            {
                urunlerimiz = db.urunler.Where(x => id == null || x.kate_no == id).OrderBy(x => x.fiyati).ToList().ToPagedList(sayfa_no, 6);
                Session["filtre"] = 3;
            }
            else
            {
                urunlerimiz = db.urunler.Where(x => id == null || x.kate_no == id).OrderByDescending(x => x.fiyati).ToList().ToPagedList(sayfa_no, 6);
                Session["filtre"] = 4;
            }
            return View(urunlerimiz);
        }

        // GET: urunlers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            urunler urunler = await db.urunler.FindAsync(id);
            if (urunler == null)
            {
                return HttpNotFound();
            }
            return View(urunler);
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
