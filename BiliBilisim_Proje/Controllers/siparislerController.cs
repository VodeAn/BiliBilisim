using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using CrystalDecisions.CrystalReports.Engine;
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
                    db.SaveChanges();
                }

                msj = "Sipariş Kaydı Yapıldı.Sipariş No:  " + sip_nosu;
                sepet_urun.Clear();
            }
            else return RedirectToAction("error", "home");
            return RedirectToAction("sepeti_goster", "sepet", new { msj });
        }
       
        public ActionResult siparis_goster()
        {
            if(Session["uye"] == null || !((uyeler)Session["uye"]).vip) return RedirectToAction("error", "home");
            var uye_id = ((uyeler)Session["uye"]).uye_id;
            var siparis = db.siparisler.Where(x => x.uye_id == uye_id) .GroupBy(x => x.sip_no);
            return View(siparis);
        }
        public ActionResult urunu_cikar(int id)
        {
            if (Session["uye"] == null || !((uyeler)Session["uye"]).vip) return RedirectToAction("error", "home");
            db.siparisler.Remove(db.siparisler.Find(id));
            db.SaveChanges();
            return RedirectToAction("siparis_goster");
        }
        public ActionResult sip_iptal(int id)
        {
            if (Session["uye"] == null || !((uyeler)Session["uye"]).vip) return RedirectToAction("error", "home");
            db.siparisler.RemoveRange(db.siparisler.Where(x => x.sip_no == id));
            db.SaveChanges();
            return RedirectToAction("siparis_goster");
        }
        public ActionResult admin_siparis_raporu(int? uyeid,DateTime? tarih1,DateTime? tarih2)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "TÜMÜ",
                    Value = "-1"
                    
                }
                
            };
            foreach (var item in db.uyeler)
            {
                SelectListItem eleman = new SelectListItem()
                {
                    Text=item.ad_soyad,
                    Value=item.uye_id.ToString()
                };
                selectListItems.Add(eleman);
            }
            ViewBag.uyeid = selectListItems;
            IQueryable siparis = null;

            if (uyeid == null && tarih1 == null && tarih2 == null)
            {
                siparis = null;
            }
            else if(uyeid != -1 && tarih2 == null)
            {
                siparis = db.siparisler.Where(x => x.uye_id == uyeid).GroupBy(x => x.sip_no);
            }
            else if (uyeid != -1 && tarih2 != null)
            {
                siparis = db.siparisler.Where(x => x.uye_id == uyeid && x.a_tarih >= tarih1 && x.a_tarih <= tarih2).GroupBy(x => x.sip_no);
            }
            else if (uyeid == -1 && tarih2 != null)
            {
                siparis = db.siparisler.Where(x => x.a_tarih >= tarih1 && x.a_tarih <= tarih2).GroupBy(x => x.sip_no);
            }
            else if (uyeid == -1 && tarih2 == null)
            {
                siparis = db.siparisler.GroupBy(x => x.sip_no);
            }
            Session["uyeid"] = uyeid;
            Session["tarih1"] = tarih1;
            Session["tarih2"] = tarih2;

            return View(siparis);
        }
        /* public ActionResult admin_sip_raporu_export(byte id)
         {
             int uyeid = Convert.ToInt32(Session["uyeid"]);
             DateTime tarih1 = Convert.ToDateTime(Session["tarih1"]);
             DateTime tarih2 = Convert.ToDateTime(Session["tarih2"]);
             IQueryable siparislerimiz = null;


             if (uyeid == -1 && Session["tarih2"] == null)
             {



                 siparislerimiz = from uye in db.uyeler
                          from urun in db.urunler
                          from sip in db.siparisler
                          where uye.uye_id == sip.uye_id && sip.urun_id == urun.urun_id
                          orderby uye.adsoyad 
                          select new
                          {
                              uye.adsoyad,
                              sip.sipno,
                              sip.sip_tarihi,
                              sip.adet,
                              urun.fiyat,
                              urun.urunadi



                          };
             }
             else if (uyeid != -1 && Session["tarih2"] != null)
             {
                 siparislerimiz = from uye in db.uyeler
                                  from urun in db.urunler
                                  from sip in db.siparisler
                                  where uye.uye_id == sip.uye_id && sip.urun_id == urun.urun_id && uye.uye_id == uyeid && 
                                  sip.a_tarih >= tarih1 && sip.a_tarih <= tarih2
                                  orderby uye.ad_soyad
                                  select new
                                  {
                                      uye.ad_soyad,
                                      sip.sip_no,
                                      sip.a_tarih,
                                      sip.adet,
                                      urun.fiyati,
                                      urun.urun_adi



                                  };
             }
             else if (uyeid == -1 && Session["tarih2"] != null)
             {
                 siparislerimiz = from uye in db.uyeler
                                  from urun in db.urunler
                                  from sip in db.siparisler
                                  where uye.uye_id == sip.uye_id && sip.urun_id == urun.urun_id &&
                                  sip.a_tarih >= tarih1 && sip.a_tarih <= tarih2
                                  orderby uye.ad_soyad
                                  select new
                                  {
                                      uye.ad_soyad,
                                      sip.sip_no,
                                      sip.a_tarih,
                                      sip.adet,
                                      urun.fiyati,
                                      urun.urun_adi



                                  };
             }
             else
             {
                 siparislerimiz = from uye in db.uyeler
                                  from urun in db.urunler
                                  from sip in db.siparisler
                                  where uye.uye_id == sip.uye_id && sip.urun_id == urun.urun_id && 
                                  uye.uye_id == uyeid
                                  orderby uye.adsoyad
                                  select new
                                  {
                                      uye.adsoyad,
                                      sip.sipno,
                                      sip.sip_tarihi,
                                      sip.adet,
                                      urun.fiyat,
                                      urun.urunadi



                                  };
             }




              ReportDocument reportDocument = new ReportDocument();
             reportDocument.Load(Server.MapPath("~/crystal_reports/siparis_report.rpt"));
             reportDocument.SetDataSource(siparislerimiz);
             Response.Buffer = false;
             Response.ClearContent();
             if (id == 1)
             {
                 Stream stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                 stream.Seek(0, SeekOrigin.Begin);
                 return File(stream, "application/pdf", "siparis_report.pdf");
             }
             else
             {
                 Stream stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                 stream.Seek(0, SeekOrigin.Begin);
                 return File(stream, "application/xls", "siparis_report.xls");
             }
         }*/

       
    }
}