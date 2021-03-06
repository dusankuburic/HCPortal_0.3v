﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HCPortal.Models;
using HCPortal.Models.L2SRepository;
using HCPortal.Models.RepositoryInterfaces;

namespace HCPortal.Controllers
{
    [Filters.ModeratorFilter]
    public class DnevnikController : Controller
    {
        private IDnevnik dnevnikRepository = new DnevnikRepository();
        private IUcenik ucenikRepositroy = new UcenikRepository();
        private IPredmet predmetRepository = new PredmetRepository();

        // GET: Dnevnik
        public ActionResult Index()
        {
            return View("Index");
        }


        public ActionResult PregledOdeljenja(int? razred)
        {

            if(razred != null)
            {
                List<OdeljenjeLOC> sva_odeljenja = dnevnikRepository.odeljenjaSaRazredom(razred);

                if (sva_odeljenja != null)
                {
                    return View("PregledOdeljenja", sva_odeljenja);
                }
                else
                {
                    return HttpNotFound("404");
                }
            }
            else
            {
                return HttpNotFound("404");
            }


        }

        public ActionResult PregledUcenika(int? odeljenje)
        {

            if(odeljenje != null)
            {
                List<UcenikLOC> svi_ucenici = dnevnikRepository.uceniciIzOdeljenja(odeljenje);

                if (svi_ucenici != null)
                {
                    return View("PregledUcenika", svi_ucenici);
                }
                else
                {
                    return HttpNotFound("404");
                }
            }
            else
            {
                return HttpNotFound("404");
            }

        }


        public ActionResult PregledPredmeta(int? ucenik_sifra)
        {

            if(ucenik_sifra != null)
            {
                List<PredmetLOC> svi_predmeti = dnevnikRepository.predmetiUcenika(ucenik_sifra);

                if (svi_predmeti != null)
                {
                    ViewBag.sifra_ucenika = ucenik_sifra;
                    var ucenikLoc = ucenikRepositroy.traziUcenika(ucenik_sifra);

                    if (ucenikLoc != null)
                    {
                        ViewBag.ucenik = ucenikLoc;
                        return View("PregledPredmeta", svi_predmeti);
                    }
                    else
                    {
                        return HttpNotFound("404");
                    }
                }
                else
                {
                    return HttpNotFound("404");
                }
            }
            else
            {
                return HttpNotFound("404");
            }
         
        }

        public ActionResult PregledOcena(int? ucenik_sifra, int? predmet_sifra)
        {
            List<OcenaUcenikaMetaLOC> sve_ocene_ucenika = dnevnikRepository.traziOceneUcenika(ucenik_sifra, predmet_sifra);

            if(sve_ocene_ucenika.Count() != 0)
            {
                ViewBag.ucenik = ucenikRepositroy.traziUcenika(ucenik_sifra);
                ViewBag.sifra_predmeta = predmet_sifra;
                ViewBag.predmet = predmetRepository.traziPredmet(predmet_sifra);
                return View("PregledOcena", sve_ocene_ucenika);
            }
            else
            {
                return HttpNotFound("404");
            }            
        }

   
        [HttpPost]
        public ActionResult UpisOcene(OcenaUcenikaMetaLOC ocenaUcenikaMetaLoc)
        {
            bool rezultat_upisa = dnevnikRepository.upisOceneUceniku(ocenaUcenikaMetaLoc);

            if(rezultat_upisa)
            {
                Session["rezultat_upisa"] = "prosao";
            }
            else
            {
                Session["rezultat_upisa"] = "pao";
            }
           


            return RedirectToAction("PregledOcena", new { ucenik_sifra = ocenaUcenikaMetaLoc.sifra_ucenika, predmet_sifra = ocenaUcenikaMetaLoc.sifra_predmeta});
        }
        
    }
}