using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class Resource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        [MaxLength(40)]
        public string Name { get; set; }
        [Required]
        [MaxLength(24)]
        public string Extension { get; set; }
        [MaxLength(8)]
        public string BodyBase64 { get; set; }
        [Required]
        [MaxLength(32)]
        public string FolderName { get; set; }
        [Required]
        [MaxLength(256)]
        public string SrcPath { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEdit { get; set; }

        public Resource()
        {
        }

    }
}
