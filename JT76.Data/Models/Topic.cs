using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using JT76.Data.Abstract;

namespace JT76.Data.Models
{
    public class Topic : ModelBase
    {
        [Required]
        [ScaffoldColumn(true)]
        [StringLength(100, MinimumLength = 5)]
        [DisplayName("Title")]
        public string StrTitle { get; set; }

        [Required]
        [ScaffoldColumn(true)]
        [StringLength(2000, MinimumLength = 15)]
        [CustomAttributes.CleanedHtmlString]
        public string StrBody { get; set; }

        //Handles the foreign key creation, requires a naming convention in Reply.cs
        public List<Reply> Replies { get; set; }
    }
}