using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BiliBilisim_Proje.Models;

namespace BiliBilisim_Proje.Models
{
    public class Favoriler
    {
        List<urunler> favoriler = new List<urunler>();

        public List<urunler> Favorilerim { get => favoriler; }

        public void favori_ekle(urunler favori)
        {
            var mevcut = favoriler.FirstOrDefault(x => x.urun_id == favori.urun_id);
            if (mevcut == null)
            {
                favoriler.Add(favori);
            }
            
        }
        public void favori_sil(urunler favori)
        {
            favoriler.RemoveAll(x => x.urun_id == favori.urun_id);
        }
    }
}