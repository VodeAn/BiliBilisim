using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BiliBilisim_Proje
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "UrunlersRoutu",
                url: "urunler",
                defaults: new {controller = "Urunlers",action = "Index"}
            );
            routes.MapRoute(
                name: "UrunDetayRoutu",
                url: "urun/detay/{id}/{kateno}",
                defaults: new {controller = "urunlers",action = "Details",kateno = UrlParameter.Optional}
            );
            routes.MapRoute(
                name: "AboutRoutu",
                url: "hakkimizda",
                defaults: new {controller = "home", action = "about" }
            );
            routes.MapRoute(
                name: "ContactRoutu",
                url: "iletisim",
                defaults: new {controller = "home", action = "Contact" }
            );
            routes.MapRoute(
                name: "Kaydol_GirisRoutu",
                url: "uye-platformu",
                defaults: new {controller = "home", action = "Kaydol_Giris" }
            );
            routes.MapRoute(
                name: "UyelersEditRoutu",
                url: "uye-platformu/hesabim",
                defaults: new {controller = "Uyelers", action = "Edit"}
            );










            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
