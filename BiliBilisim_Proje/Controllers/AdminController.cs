using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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
        public ActionResult AdminUyeCreate()
        {
            if (Session["admin"] == null) return HttpNotFound();
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(
                    new SelectListItem()
                    {
                        Text = "Erkek",
                        Value = false.ToString()
                    });
            selectListItems.Add(
                    new SelectListItem()
                    {
                        Text = "Kadın",
                        Value = true.ToString()
                    });

            ViewBag.cinsiyet = selectListItems;

                
            ViewBag.plaka = new SelectList(dbo.sehirler, "plaka", "il");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminUyeCreate([Bind(Include = "uye_id,kuladi,sifre,ad_soyad,dog_tar,cinsiyet,vip,adres,email,plaka")] uyeler uyeler)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbo.uyeler.Add(uyeler);
                    await dbo.SaveChangesAsync();
                    TempData["KayitBasarili"] = "Üye Başarıyla Kaydedildi ";
                    return RedirectToAction("UyeListele");
                }
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems.Add(
                        new SelectListItem()
                        {
                            Text = "Erkek",
                            Value = false.ToString()
                        });
                selectListItems.Add(
                        new SelectListItem()
                        {
                            Text = "Kadın",
                            Value = true.ToString()
                        });

                ViewBag.cinsiyet = selectListItems;
                ViewBag.plaka = new SelectList(dbo.sehirler, "plaka", "il", uyeler.plaka);

                return View(uyeler);
            }
            catch (Exception error)
            {
                string msj = error.GetBaseException().Message;
                if (msj.Contains("uyeler_email_key")) ModelState.AddModelError("email", "Bu email zaten mevcut");
                else if (msj.Contains("uyeler_kuladi_key")) ModelState.AddModelError("kuladi", "Bu kullanıcı adı zaten mevcut");
                else TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems.Add(
                        new SelectListItem()
                        {
                            Text = "Erkek",
                            Value = false.ToString()
                        });
                selectListItems.Add(
                        new SelectListItem()
                        {
                            Text = "Kadın",
                            Value = true.ToString()
                        });

                ViewBag.cinsiyet = selectListItems;


                ViewBag.plaka = new SelectList(dbo.sehirler, "plaka", "il");
                return View(uyeler);
            };

        }

        public async Task<ActionResult> AdminUyeEdit(int? id)
        {
            if (Session["admin"] == null)
            {

                return HttpNotFound();
            }
            if (id == null)
            {
                return HttpNotFound();
            }
            uyeler uyeler = await dbo.uyeler.FindAsync(id);
            if (uyeler == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(
                    new SelectListItem()
                    {
                        Text = "Erkek",
                        Value = false.ToString()
                    });
            selectListItems.Add(
                    new SelectListItem()
                    {
                        Text = "Kadın",
                        Value = true.ToString()
                    });

            ViewBag.cinsiyet = selectListItems;

            ViewBag.plaka = new SelectList(dbo.sehirler, "plaka", "il", uyeler.plaka);
            return View(uyeler);
        }
        [HttpPost]
        public async Task<ActionResult> AdminUyeEdit(uyeler uyeler)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (uyeler == null) return HttpNotFound();
                    dbo.Entry(uyeler).State = EntityState.Modified;
                    await dbo.SaveChangesAsync();
                    TempData["KayitBasarili"] = "Üye Başarıyla Güncellendi.";
                    return RedirectToAction("UyeListele", "Admin");

                }
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems.Add(
                        new SelectListItem()
                        {
                            Text = "Erkek",
                            Value = false.ToString()
                        });
                selectListItems.Add(
                        new SelectListItem()
                        {
                            Text = "Kadın",
                            Value = true.ToString()
                        });

                ViewBag.cinsiyet = selectListItems;


                ViewBag.plaka = new SelectList(dbo.sehirler, "plaka", "il");
                return View(uyeler);
            }
            catch(Exception error)
            {
                string msj = error.GetBaseException().Message;
                if (msj.Contains("uyeler_email_key")) ModelState.AddModelError("email", "Bu email zaten mevcut");
                else if (msj.Contains("uyeler_kuladi_key")) ModelState.AddModelError("kuladi", "Bu kullanıcı adı zaten mevcut");
                else TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                selectListItems.Add(
                        new SelectListItem()
                        {
                            Text = "Erkek",
                            Value = false.ToString()
                        });
                selectListItems.Add(
                        new SelectListItem()
                        {
                            Text = "Kadın",
                            Value = true.ToString()
                        });

                ViewBag.cinsiyet = selectListItems;


                ViewBag.plaka = new SelectList(dbo.sehirler, "plaka", "il");
                return View(uyeler);
            };
           

        }

        public async Task<ActionResult> AdminUyeDelete(int? id)
        {
            if (Session["admin"] == null) return HttpNotFound();
            if (id == null)
            {
                return HttpNotFound();
            }
            uyeler uyeler = await dbo.uyeler.FindAsync(id);
            if (uyeler == null)
            {
                return HttpNotFound();
            }
            dbo.uyeler.Remove(uyeler);
            await dbo.SaveChangesAsync();
            TempData["KayitBasarili"] = "Üye Başarıyla Silindi.";
            return RedirectToAction("UyeListele","Admin");
        }
      

        //-----------------------------------------------Kategori Bölümü---------------------------------------------------------------------
        public ActionResult KategoriListele(int? sayfa)
        {
            if (Session["admin"] == null) return HttpNotFound();
            int sayfa_no = sayfa ?? 1;
            var kategori = dbo.kategori.GroupBy(x => x.ust_kategori.u_kate_adi).OrderBy(x => x.Key);
            return View(kategori.ToPagedList(sayfa_no, 1));
        }
        public ActionResult AdminKateCreate()
        {
            if (Session["admin"] == null) return HttpNotFound();
            ViewBag.UstKategoriListesi = new SelectList(dbo.ust_kategori, "u_kate_id", "u_kate_adi");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminKateCreate(kategori kategori)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbo.kategori.Add(kategori);
                    await dbo.SaveChangesAsync();
                    TempData["KayitBasarili"] = "Kategori Başarıyla Kaydedildi ";
                    return RedirectToAction("KategoriListele");
                }
                ViewBag.UstKategoriListesi = new SelectList(dbo.ust_kategori, "u_kate_id", "u_kate_adi");
                return View(kategori);
            }
            catch(Exception error)
            {
                string msj = error.GetBaseException().Message;
                if (msj.Contains("IX_kategori")) ModelState.AddModelError("kate_adi", "Bu kategori zaten mevcut");
                else TempData["KayitBasarili"] = "Bilinmeyen Bİr Hata Oluştu";
                return View(kategori);
            }
         
        }
        public async Task<ActionResult> AdminKateEdit(int? id)
        {
            if (Session["admin"] == null)
            {

                return HttpNotFound();
            }
            if (id == null)
            {
                return HttpNotFound();
            }
            kategori kategori = await dbo.kategori.FindAsync(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            ViewBag.UstKategoriListesi = new SelectList(dbo.ust_kategori, "u_kate_id", "u_kate_adi");
            return View(kategori);
        }
        [HttpPost]
        public async Task<ActionResult> AdminKateEdit(kategori kategori)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (kategori == null) return HttpNotFound();
                    dbo.Entry(kategori).State = EntityState.Modified;
                    await dbo.SaveChangesAsync();
                    TempData["KayitBasarili"] = "Kategori Başarıyla Güncellendi.";
                    return RedirectToAction("KategoriListele", "Admin");
                }
                ViewBag.UstKategoriListesi = new SelectList(dbo.ust_kategori, "u_kate_id", "u_kate_adi");
                return View(kategori);
            }
            catch(Exception error)
            {
                string msj = error.GetBaseException().Message;
                if (msj.Contains("IX_kategori")) ModelState.AddModelError("kate_adi", "Bu kategori zaten mevcut");
                else TempData["KayitBasarili"] = "Bilinmeyen Bİr Hata Oluştu";
                return View(kategori);
            }
           


        }
        public async Task<ActionResult> AdminKateDelete(int? id)
        {
            if (Session["admin"] == null) return HttpNotFound();
            if (id == null)
            {
                return HttpNotFound();
            }
            kategori kategori = await dbo.kategori.FindAsync(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            dbo.kategori.Remove(kategori);
            await dbo.SaveChangesAsync();
            TempData["KayitBasarili"] = "Kategori Başarıyla Silindi.";
            return RedirectToAction("KategoriListele", "Admin");
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