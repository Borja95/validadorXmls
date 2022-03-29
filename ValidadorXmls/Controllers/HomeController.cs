using SAT.Services.ConsultaCFDIService;
using SW.Services.Status;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using ValidadorXmls.Models;

namespace ValidadorXmls.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult ValidarXMLCargar(HttpPostedFileBase[] archivos)
        {
            List<CFDI> xmls = new List<CFDI>();

            for (int i = 0; i < archivos.Length; i++)
            {
                CFDI xmlActual = new CFDI();
                byte[] archivoActual = ConvertToBytes(archivos[i]);
                Stream stream = new MemoryStream(archivoActual);
                XmlTextReader xmlReader = new XmlTextReader(stream);

                while (xmlReader.Read())
                {
                    if(xmlReader.NodeType==XmlNodeType.Element && (xmlReader.Name == "cfdi:Emisor"))
                    {
                        if (xmlReader.HasAttributes)
                            xmlActual.RFC_EMISOR = xmlReader.GetAttribute("Rfc")==null ? xmlReader.GetAttribute("rfc") : xmlReader.GetAttribute("Rfc");
                    }

                    if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.Name == "cfdi:Receptor"))
                    {
                        if (xmlReader.HasAttributes)
                            xmlActual.RFC_RECEPTOR = xmlReader.GetAttribute("Rfc") == null ? xmlReader.GetAttribute("rfc") : xmlReader.GetAttribute("Rfc");
                    }

                    if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.Name == "cfdi:Emisor"))
                    {
                        if (xmlReader.HasAttributes)
                            xmlActual.RFC_EMISOR = xmlReader.GetAttribute("Rfc") == null ? xmlReader.GetAttribute("rfc") : xmlReader.GetAttribute("Rfc");
                    }
                    
                    if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.Name == "cfdi:Comprobante"))
                    {
                        if (xmlReader.HasAttributes)
                        {
                            xmlActual.TOTAL = xmlReader.GetAttribute("Total") == null ? xmlReader.GetAttribute("total") : xmlReader.GetAttribute("Total");
                            xmlActual.SERIE = xmlReader.GetAttribute("Serie") == null ? xmlReader.GetAttribute("serie") : xmlReader.GetAttribute("Serie");
                            xmlActual.FOLIO = xmlReader.GetAttribute("Folio") == null ? xmlReader.GetAttribute("folio") : xmlReader.GetAttribute("Folio");
                        }
                    }

                    if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.Name == "tfd:TimbreFiscalDigital"))
                    {
                        if (xmlReader.HasAttributes)
                            xmlActual.UUID = xmlReader.GetAttribute("UUID");
                    }
                }

                Status status = new Status("https://consultaqr.facturaelectronica.sat.gob.mx/ConsultaCFDIService.svc");
                Acuse response = status.GetStatusCFDI(xmlActual.RFC_EMISOR, xmlActual.RFC_RECEPTOR, xmlActual.TOTAL, xmlActual.UUID);
                xmlActual.CODIGO_ESTATUS = response.CodigoEstatus;
                xmlActual.ESTADO = response.Estado;
                xmlActual.ES_CANCELABLE = response.EsCancelable;
                xmlActual.ESTATUS_CANCELACION = response.EstatusCancelacion;

                xmls.Add(xmlActual);
            }

            return Json(xmls);
        }

        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            int fileSizeInBytes = file.ContentLength;
            byte[] data = null;
            using (var br = new BinaryReader(file.InputStream))
            {
                data = br.ReadBytes(fileSizeInBytes);
            }

            return data;
        }
    }
}