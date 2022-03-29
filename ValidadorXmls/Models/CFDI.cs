using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidadorXmls.Models
{
    public class CFDI
    {
        public string SERIE { get; set; }
        public string FOLIO { get; set; }
        public string RFC_EMISOR { get; set; }
        public string RFC_RECEPTOR { get; set; }
        public string TOTAL { get; set; }
        public string UUID { get; set; }
        public string ESTADO { get; set; }
        public string CODIGO_ESTATUS { get; set; }
        public string ES_CANCELABLE { get; set; }
        public string ESTATUS_CANCELACION { get; set; }
    }
}