using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiliBilisim_Proje.Models
{
    public class Sepet
    {
        List<Sepetlik> sepetim = new List<Sepetlik>();

        public List<Sepetlik> Sepetim { get => sepetim;  }

        public void sepet_ekle(urunler sepete_eklenecek,int adet)
        {
            var sepette_bulunan = sepetim.FirstOrDefault(x => x.urun.urunid == sepete_eklenecek.urunid);
            if (sepette_bulunan == null)
            {
                sepetim.Add(new Sepetlik() { urun = sepete_eklenecek, adet = 1 });
            }
            else if (adet == 0) sepette_bulunan.adet++;
            else sepette_bulunan.adet = adet;
        }
        public void sepetten_sil(urunler sepetten_silinecek)
        {
            sepetim.RemoveAll(x => x.urun.urunid == sepetten_silinecek.urunid);
        }
        public void sepetten_temizle()
        {
            sepetim.Clear();
        }
        public double sepeti_topla()
        {
            return sepetim.Sum(x => x.adet * x.urun.fiyat);
        }
    }
}