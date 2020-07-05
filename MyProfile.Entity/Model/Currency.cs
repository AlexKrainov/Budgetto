using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    /// <summary>
    /// https://www.xe.com/currency/gbp-british-pound
    /// Insert into dbo.Currencies ([Name], CodeName,  Icon, SpecificCulture, CanBeUser)
    ///values('United Kingdom Pound', 'GBP', NCHAR(163),'en-GB',0)
    /// </summary>
    public class Currency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(24)]
        public string Name { get; set; }
        [Required]
        [MaxLength(3)]
        public string CodeName { get; set; }
        [Required]
        [MaxLength(16)]
        public string SpecificCulture { get; set; }
        [Required]
        [MaxLength(1)]
        public string Icon { get; set; }
        /// <summary>
        /// Show or hide, as in the choice for the user, as the main currency
        /// </summary>
        public bool CanBeUser { get; set; }
        [MaxLength(8)]
        /// <summary>
        /// Name carrency for http://www.cbr.ru/development/sxml/
        /// </summary>
        public string CodeName_CBR { get; set; }
        public int? CodeNumber_CBR { get; set; }
    }
}
