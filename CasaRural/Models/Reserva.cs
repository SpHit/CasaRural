using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CasaRural.Models
{
    public class Reserva
    {
        [Key]
        public int ReservaId { get; set; }
        [Display(Name = "Data entrada")]
        public DateTime DataEntrada { get; set; }
        [Display(Name = "Data sortida")]
        public DateTime? DataSortida { get; set; }

        [Display(Name = "Llogater")]
        public int LlogaterId { get; set; }
        public virtual Llogater Llogater { get; set; }
    }
}