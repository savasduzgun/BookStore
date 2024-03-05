using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Entities
{
    public class Slide : EntityBase
    {
        [DisplayName("Adı")]
        [Required(ErrorMessage = "Adı alanı zorunludur")]
        [MaxLength(50, ErrorMessage = "Adı alanı maksimum 50 karakter olmalıdır")]
        public string Name { get; set; }

        [DisplayName("Sıra No")]
        [Required(ErrorMessage = "Sıra No alanı zorunludur")]
        public int OrderNo { get; set; }

        [DisplayName("Slayt")]
        [MaxLength(250, ErrorMessage = "Slayt alanı maksimum 250 karakter olmalıdır")]
        public string? SlideUrl { get; set; }

        [DisplayName("URL")]
        [MaxLength(250, ErrorMessage = "URL alanı maksimum 250 karakter olmalıdır")]
        public string? Url { get; set; }

        [DisplayName("Slayt aktif mi ?")]
        public bool IsActive { get; set; }
    }
}
