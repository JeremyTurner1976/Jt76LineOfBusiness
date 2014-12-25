using System.ComponentModel.DataAnnotations;
using JT76.Data.Abstract;

namespace JT76.Data.Models
{
    public class Reply : ModelBase
    {
        [Required]
        [ScaffoldColumn(true)]
        [StringLength(2000, MinimumLength = 15)]
        [CustomAttributes.CleanedHtmlString]
        public string StrBody { get; set; }

        //will automatically create the foreign key because named as Storing class
        //MUST be named like this
        public int TopicId { get; set; }
    }
}