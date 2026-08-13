using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BiliBilisim_Proje.Models;

namespace MVC5_E_Ticaret.Controllers
{
    public class siparislerController : Controller
    {
        // GET: spiarisler
       bili_Entities db = new bili_Entities();
        public ActionResult siparis_gec()
        {
            string msj = "";
            if (Session["uye"] != null || ((uyeler)Session["uye"]).vip)
            {
                var sip_nosu = db.siparisler.Max(x => x.sip_no) + 1;
                var sepet_urun = ((Sepet)Session["sepet"]).Sepetim;
                if (sepet_urun.Count==0)
                {
                    msj = "Sepetiniz boş lütfen sepete ürün ekleyin!";
                }
                else
                {

              
                    foreach (var item in sepet_urun)
                    {
                        siparisler siparisler = new siparisler()
                        {
                            urun_id = item.urun.urun_id,
                            sip_no = sip_nosu,
                            a_tarih = DateTime.Now,
                            adet = Convert.ToInt16(item.adet),
                            uye_id = ((uyeler)Session["uye"]).uye_id
                        };
                        db.siparisler.Add(siparisler);
                        var satinalinan = db.urunler.Find(item.urun.urun_id);
                        if (satinalinan != null)
                        {
                            satinalinan.stok -= item.adet;
                            satinalinan.satis_adet += Convert.ToInt16(item.adet);
                            if (satinalinan.stok < 0) satinalinan.stok = 0;
                        }

                        db.SaveChanges();
                    }

                msj = "Sipariş Kaydı Yapıldı.Sipariş No:  " + sip_nosu;
                sepet_urun.Clear();
                }
            }
            else return RedirectToAction("error", "home");
            return RedirectToAction("sepeti_goster", "sepet", new { msj });
        }
       
        public ActionResult siparis_goster(int? tarihler)
        {
            if(Session["uye"] == null || !((uyeler)Session["uye"]).vip) return RedirectToAction("error", "home");
           List<SelectListItem> selectListItem = new List<SelectListItem>();
            for(int i = DateTime.Now.Year; i >= 1900; i--)
            {
                selectListItem.Add(
                        new SelectListItem()
                        {
                            Value = i.ToString(),
                            Text = i.ToString()

                        });
               
            }
            var uye_id = ((uyeler)Session["uye"]).uye_id;
            var siparis = db.siparisler.Where(x => x.uye_id == uye_id) .GroupBy(x => x.sip_no);
            if(tarihler != null)
            {
                siparis = db.siparisler.Where(x => x.uye_id == uye_id && x.a_tarih.Year == tarihler).GroupBy(x => x.sip_no);
            }
            else
            {
                siparis = db.siparisler.Where(x => x.uye_id == uye_id).GroupBy(x => x.sip_no);
            }
            if (tarihler == null)
            {
                ViewBag.siparisad = "";
            }
           else if (siparis.Count() == 0)
            {
                ViewBag.siparisad = tarihler.ToString() + " Yılına ait siparişiniz bulunamadı";
            }
            else
            {
                ViewBag.siparisad = tarihler.ToString() + " Yılına ait " + siparis.Count() + " Adet siparişiniz bulundu";
            }

            ViewBag.tarihler = selectListItem;
            return View(siparis);
        }
        public ActionResult urunu_cikar(int id)
        {
            if (Session["uye"] == null || !((uyeler)Session["uye"]).vip) return RedirectToAction("error", "home");
            var seciliurun = db.siparisler.Find(id);
           
            var iadeurun = db.urunler.Find(seciliurun.urun_id);
            if (iadeurun != null)
            {
                iadeurun.stok += seciliurun.adet;
                iadeurun.satis_adet -= Convert.ToInt16(seciliurun.adet);
                if (iadeurun.satis_adet < 0) iadeurun.satis_adet = 0;
            }
             db.siparisler.Remove(seciliurun);
            db.SaveChanges();
            string msj = "Ürün Başarıyla İade Edildi.Ana Sayfaya Yönlendiriliyorsunuz...";
            return RedirectToAction("iptal_sayfasi","siparisler",new {msj });
        }
        public ActionResult sip_iptal(int id)
        {
            if (Session["uye"] == null || !((uyeler)Session["uye"]).vip) return RedirectToAction("error", "home");
            var iptalurun = db.siparisler.Where(x => x.sip_no == id).ToList();
            foreach (var item in iptalurun)
            {
                if(db.urunler.Any(x=>x.urun_id == item.urun_id))
                {
                    var urun = db.urunler.Find(item.urun_id);
                    if (urun != null)
                    {
                        urun.stok += item.adet;
                        urun.satis_adet -= Convert.ToInt16(item.adet);
                        if (urun.satis_adet < 0) urun.satis_adet = 0;
                    }
                        
                }
            }
            db.siparisler.RemoveRange(iptalurun);
            db.SaveChanges();
            string msj = "Sipariş Başarıyla İptal Edildi.Ana Sayfaya Yönlendiriliyorsunuz.";
            return RedirectToAction("iptal_sayfasi", "siparisler", new { msj });
        }
        
       public ActionResult iptal_sayfasi(string msj)
        {
            ViewBag.msj = msj;
            Response.AddHeader("Refresh", "2; url=" + Url.Action("Index", "Home"));
            return View();
        }
       
    }
}