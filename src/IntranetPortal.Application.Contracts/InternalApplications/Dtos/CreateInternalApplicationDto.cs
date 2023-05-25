using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IntranetPortal.InternalApplicationDto
{
    public class CreateInternalApplicationDto
    {
        public string Description { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string? DisplayName { get; set; }
        [Required]

        public bool IsActive { get; set; }
        [Required]

        public int OrderNumber { get; set; }
        public string? LogoUrl { get; set; }
        public string? ApplicationUrl { get; set; }
    }
}
