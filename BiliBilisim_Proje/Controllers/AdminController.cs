using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
            try
            {
                var admin = dbo.admin.FirstOrDefault(x => x.kuladi == username && x.sifre == password);
                if (admin != null)
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
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu.";
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
            try
            {
                if (Session["admin"] == null) return HttpNotFound();
                int sayfa_no = sayfa ?? 1;
                IPagedList<uyeler> uyeler = dbo.uyeler.OrderBy(x => x.ad_soyad).ToPagedList(sayfa_no, 3);
                return View(uyeler);
            }
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu.";
                return RedirectToAction("Index","Admin");
            }
            
        }
        public ActionResult AdminUyeCreate()
        {
            try
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
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                return RedirectToAction("Index", "Admin");
            }

          
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
            try
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
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                return RedirectToAction("Index", "Admin");
            }
           
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
            try
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
                return RedirectToAction("UyeListele", "Admin");
            }
            catch (Exception error)
            {
                string msj = error.GetBaseException().Message;
                if (msj.Contains("siparisler_uye_id_fkey")) TempData["KayitBasarili"] = "Siparişi Olan Üye Silinemez.";
                else TempData["KayitBasarili"] = "Bilinmeyen bir hata oluştu";
                return RedirectToAction("AdminUyeEdit", "Admin", new { id });
            }

        }
            

        //-----------------------------------------------Kategori Bölümü---------------------------------------------------------------------
        public ActionResult KategoriListele(int? sayfa)
        {
            try
            {
                if (Session["admin"] == null) return HttpNotFound();
                int sayfa_no = sayfa ?? 1;
                var kategori = dbo.kategori.GroupBy(x => x.ust_kategori.u_kate_adi).OrderBy(x => x.Key);
                return View(kategori.ToPagedList(sayfa_no, 1));
            }
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu.";
                return RedirectToAction("Index", "Admin");
            }
        
        }
        public ActionResult AdminKateCreate()
        {
            try
            {
                if (Session["admin"] == null) return HttpNotFound();
                ViewBag.UstKategoriListesi = new SelectList(dbo.ust_kategori, "u_kate_id", "u_kate_adi");
                return View();
            }
             catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                return RedirectToAction("KategoriListele", "Admin");
            }
          
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
                else TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                return View(kategori);
            }
         
        }
        public async Task<ActionResult> AdminKateEdit(int? id)
        {
            try
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
            catch(Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                return RedirectToAction("Index","Admin");
            }
            
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
                else ModelState.AddModelError("kate_adi", "Bilinmeyen bir hata oluştu");
                return View(kategori);
            }
           


        }
        public async Task<ActionResult> AdminKateDelete(int? id)
        {
            try
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
            catch(Exception error)
            {
                string msj = error.GetBaseException().Message;
                if (msj.Contains("FK_urunler_kategori")) TempData["KayitBasarili"] = "Bu kategoriye ait ürünler mevcut.Kategori silinemedi."; 
                else ModelState.AddModelError("kate_adi", "Bilinmeyen bir hata oluştu");
                return RedirectToAction("AdminKateEdit","Admin", new { id });
            }
          
        }
        //----------------------------------------------Siparişler Bölümü--------------------------------------------------------------------
        public ActionResult SiparisListele(int? sayfa)
        {
            try
            {
                if (Session["admin"] == null) return HttpNotFound();
                int sayfa_no = sayfa ?? 1;
                var siparisler = dbo.siparisler.GroupBy(x => x.uyeler.kuladi).OrderBy(x => x.Key);
                return View(siparisler.ToPagedList(sayfa_no, 1));
            }
             catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu.";
                return RedirectToAction("Index", "Admin");
            }
        }
       
        public async Task<ActionResult> AdminSipEdit(int? id)
        {
            try
            {
                if (Session["admin"] == null)
                {

                    return HttpNotFound();
                }
                if (id == null)
                {
                    return HttpNotFound();
                }
                siparisler siparisler = await dbo.siparisler.FindAsync(id);
                if (siparisler == null)
                {
                    return HttpNotFound();
                }
                ViewBag.UyeListesi = new SelectList(dbo.uyeler, "uye_id", "ad_soyad",siparisler.uye_id);
                ViewBag.UrunListesi = new SelectList(dbo.urunler, "urun_id", "urun_adi",siparisler.urun_id);
                return View(siparisler);
            }
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu.";
                return RedirectToAction("Index", "Admin");
            }

        }
        [HttpPost]
        public async Task<ActionResult> AdminSipEdit(siparisler siparisler)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (siparisler == null) return HttpNotFound();
                    dbo.Entry(siparisler).State = EntityState.Modified;
                    await dbo.SaveChangesAsync();
                    TempData["KayitBasarili"] = "Sipariş Başarıyla Güncellendi.";
                    return RedirectToAction("SiparisListele", "Admin");
                }
                ViewBag.uye_id = new SelectList(dbo.uyeler, "uye_id", "ad_soyad");
                ViewBag.urun_id = new SelectList(dbo.urunler, "urun_id", "urun_adi");
                return View(siparisler);
            }
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                return View(siparisler);
            }
           


        }
        public async Task<ActionResult> AdminSipDelete(int? id)
        {
            try
            {
                if (Session["admin"] == null) return HttpNotFound();
                if (id == null)
                {
                    return HttpNotFound();
                }
                siparisler siparisler = await dbo.siparisler.FindAsync(id);
                if (siparisler == null)
                {
                    return HttpNotFound();
                }
                dbo.siparisler.Remove(siparisler);
                await dbo.SaveChangesAsync();
                TempData["KayitBasarili"] = "Sipariş Başarıyla Silindi.";
                return RedirectToAction("SiparisListele", "Admin");
            }
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                return RedirectToAction("AdminSipEdit", "Admin", new { id });
            }

        }
        //----------------------------------------------Ürünler Bölümü-----------------------------------------------------------------------
        public ActionResult UrunListele(int? sayfa)
        {
            try
            {
                if (Session["admin"] == null) return HttpNotFound();
                int sayfa_no = sayfa ?? 1;
                var urunler = dbo.urunler
                                 .Include(x => x.kategori)
                                 .ToList()
                                 .GroupBy(x => x.kategori != null ? x.kategori.kate_adi : "Kategorisiz")
                                 .OrderBy(x => x.Key)
                                 .ToPagedList(sayfa_no, 1);

                return View(urunler);
            }
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu.";
                return RedirectToAction("Index", "Admin");
            }
           
        }
        public ActionResult AdminUrunCreate()
        {
            try
            {
                if (Session["admin"] == null)
                {
                    return HttpNotFound();
                }
                ViewBag.KategoriListesi = new SelectList(dbo.kategori, "kate_no", "kate_adi");
                return View(new urunler());
            }
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu";
                return RedirectToAction("UrunListele", "Admin");
            }
            
        }
        [HttpPost]
        public async Task<ActionResult> AdminUrunCreate(urunler urunler, HttpPostedFileBase dosya_adi)
        {
            if (Session["admin"] == null) return HttpNotFound();
            if (dosya_adi != null && dosya_adi.ContentLength > 0)
            {
                string uzanti = Path.GetExtension(dosya_adi.FileName).ToLower();
                if (uzanti != ".png" && uzanti != ".jpeg" && uzanti != ".jpg")
                {
                    ModelState.AddModelError("fotolar", "Sadece .png, .jpg veya .jpeg formatında dosya yükleyebilirsiniz.");
                }
            }
            else
            {
                ModelState.AddModelError("fotolar", "Lütfen bir ürün görseli seçiniz.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    int yeni_id = dbo.urunler.Any() ? dbo.urunler.Max(x => x.urun_id) + 1 : 1;
                    string resim_adi = yeni_id + "_" + Path.GetFileName(dosya_adi.FileName);
                    string klasorYolu = Server.MapPath("~/Content/fotolar/");

                    if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);

                    dosya_adi.SaveAs(Path.Combine(klasorYolu, resim_adi));
                    urunler.fotolar = resim_adi;

                    dbo.urunler.Add(urunler);
                    await dbo.SaveChangesAsync();

                    TempData["KayitBasarili"] = "Ürün Başarıyla Kaydedildi";
                    return RedirectToAction("UrunListele", "Admin");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Veritabanı kayıt hatası oluştu.");
                }
            }
            ViewBag.KategoriListesi = new SelectList(dbo.kategori.ToList(), "kate_no", "kate_adi", urunler.kate_no);
            return View(urunler);
        }
        public async Task<ActionResult> AdminUrunEdit(int? id)
        {
            try
            {
                if (Session["admin"] == null) return HttpNotFound();
                if (id == null) return HttpNotFound();

                urunler urun = await dbo.urunler.FindAsync(id);
                if (urun == null) return HttpNotFound();

                ViewBag.KategoriListesi = new SelectList(dbo.kategori.ToList(), "kate_no", "kate_adi", urun.kate_no);
                return View(urun);
            }
            catch (Exception)
            {
                TempData["KayitBasarili"] = "Bilinmeyen Bir Hata Oluştu.";
                return RedirectToAction("Index", "Admin");
            }
           
        }

        [HttpPost]
        public async Task<ActionResult> AdminUrunEdit(urunler urunler, HttpPostedFileBase dosya_adi)
        {
            if (Session["admin"] == null) return HttpNotFound();

            if (ModelState.IsValid)
            {
                if (dosya_adi != null && dosya_adi.ContentLength > 0)
                {
                    string uzanti = Path.GetExtension(dosya_adi.FileName).ToLower();
                    if (uzanti == ".png" || uzanti == ".jpeg" || uzanti == ".jpg")
                    {
                        string resim_adi = urunler.urun_id + Path.GetFileName(dosya_adi.FileName);
                        string klasorYolu = Server.MapPath("~/Content/fotolar/");

                        if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);

                        dosya_adi.SaveAs(Path.Combine(klasorYolu, resim_adi));
                        urunler.fotolar = resim_adi;
                    }
                    else
                    {
                        ModelState.AddModelError("fotolar", "Sadece .png, .jpg veya .jpeg formatında dosya yükleyebilirsiniz.");
                        ViewBag.KategoriListesi = new SelectList(dbo.kategori.ToList(), "kate_no", "kate_adi", urunler.kate_no);
                        return View(urunler);
                    }
                }
                else
                {
                    var eskiUrun = await dbo.urunler.AsNoTracking().FirstOrDefaultAsync(x => x.urun_id == urunler.urun_id);
                    if (eskiUrun != null)
                    {
                        urunler.fotolar = eskiUrun.fotolar;
                    }
                }

                dbo.Entry(urunler).State = EntityState.Modified;
                await dbo.SaveChangesAsync();

                TempData["KayitBasarili"] = "Ürün Başarıyla Güncellendi.";
                return RedirectToAction("UrunListele", "Admin");
            }

            ViewBag.KategoriListesi = new SelectList(dbo.kategori.ToList(), "kate_no", "kate_adi", urunler.kate_no);
            return View(urunler);
        }
        public async Task<ActionResult> AdminUrunDelete(int? id)
        {
            if (Session["admin"] == null) return HttpNotFound();
            if (id == null) return HttpNotFound();

            urunler urun = await dbo.urunler.FindAsync(id);
            if (urun == null) return HttpNotFound();

            try
            {
                if (!string.IsNullOrEmpty(urun.fotolar))
                {
                    string tamYol = Server.MapPath("~/Content/fotolar/" + urun.fotolar);
                    if (System.IO.File.Exists(tamYol))
                    {
                        System.IO.File.Delete(tamYol);
                    }
                }

                dbo.urunler.Remove(urun);
                await dbo.SaveChangesAsync();
                TempData["KayitBasarili"] = "Ürün ve görseli başarıyla silindi.";
            }
            catch (Exception error)
            {
                string msj = error.GetBaseException().Message;
                if (msj.Contains("siparisler_urun_id_fkey")) TempData["KayitBasarili"] = "Siparişi Olan Ürün Silinemez.";
                else TempData["KayitBasarili"] = "Ürün silinirken bir hata oluştu.";
            }

            return RedirectToAction("UrunListele", "Admin");
        }
    }
   
}
