using System.ComponentModel.DataAnnotations;
using System.Text;
using JT76.Data.Abstract;

namespace JT76.Data.Models
{
    public class Error : ModelBase
    {
        [StringLength(150)]
        public string StrMessage { get; set; }

        [StringLength(50)]
        public string StrErrorLevel { get; set; }

        [StringLength(255)]
        public string StrSource { get; set; }

        [StringLength(255)]
        public string StrAdditionalInformation { get; set; }

        [StringLength(4000)]
        [CustomAttributes.CleanedHtmlString]
        public string StrStackTrace { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(DtCreated.ToLongDateString() + " " + DtCreated.ToShortTimeString());
            sb.AppendLine(StrMessage);
            sb.AppendLine(StrSource + StrErrorLevel);
            sb.AppendLine(StrAdditionalInformation);
            sb.AppendLine(StrStackTrace);

            return sb.ToString();
        }
    }
}