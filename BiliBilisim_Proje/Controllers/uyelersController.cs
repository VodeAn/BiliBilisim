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

namespace BiliBilisim_Proje.Controllers
{
    public class uyelersController : Controller
    {
        private bili_Entities db = new bili_Entities();

        // GET: uyelers
        public async Task<ActionResult> Index()
        {
            var uyeler = db.uyeler.Include(u => u.sehirler);
            return View(await uyeler.ToListAsync());
        }

        // GET: uyelers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            uyeler uyeler = await db.uyeler.FindAsync(id);
            if (uyeler == null)
            {
                return HttpNotFound();
            }
            return View(uyeler);
        }

        // GET: uyelers/Create
        public ActionResult Create()
        {
            ViewBag.plaka = new SelectList(db.sehirler, "plaka", "il");
            return View();
        }

        // POST: uyelers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "uye_id,kuladi,sifre,ad_soyad,dog_tar,cinsiyet,vip,adres,email,plaka")] uyeler uyeler)
        {
            if (ModelState.IsValid)
            {
                db.uyeler.Add(uyeler);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.plaka = new SelectList(db.sehirler, "plaka", "il", uyeler.plaka);
            return View(uyeler);
        }

        // GET: uyelers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            uyeler uyeler = await db.uyeler.FindAsync(id);
            if (uyeler == null)
            {
                return HttpNotFound();
            }
            ViewBag.plaka = new SelectList(db.sehirler, "plaka", "il", uyeler.plaka);
            return View(uyeler);
        }

        // POST: uyelers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "uye_id,kuladi,sifre,ad_soyad,dog_tar,cinsiyet,vip,adres,email,plaka")] uyeler uyeler,string confirm)
        {
            if (ModelState.IsValid)
            {
                if (uyeler.sifre == confirm)
                {
                    db.Entry(uyeler).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else ModelState.AddModelError("KayitHata", "Şifreler birbiriyle uyuşmuyor!");

            }
            ViewBag.plaka = new SelectList(db.sehirler, "plaka", "il", uyeler.plaka);
            return View(uyeler);
        }

        // GET: uyelers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            uyeler uyeler = await db.uyeler.FindAsync(id);
            if (uyeler == null)
            {
                return HttpNotFound();
            }
            return View(uyeler);
        }

        // POST: uyelers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            uyeler uyeler = await db.uyeler.FindAsync(id);
            db.uyeler.Remove(uyeler);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
