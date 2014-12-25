using System.ComponentModel.DataAnnotations;
using JT76.Data.Abstract;

namespace JT76.Data.Models
{
    public class LogMessage : ModelBase
    {
        [Required]
        [ScaffoldColumn(true)]
        [StringLength(2000, MinimumLength = 1)]
        [CustomAttributes.CleanedHtmlString]
        public string StrLogMessage { get; set; }
    }
}