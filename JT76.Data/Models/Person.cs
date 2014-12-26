using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JT76.Data.Abstract;

namespace JT76.Data.Models
{
    public class Person : ModelBase
    {
        public Person()
        {
            Gender = " "; // make no assumption
            ImageSource = string.Empty;
        }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Url]
        public string Blog { get; set; }
        public string Twitter { get; set; }

        [StringLength(1,MinimumLength = 1) ]
        public string Gender { get; set; }
        public string ImageSource { get; set; }
        public string Bio { get; set; }

        public virtual ICollection<Session> SpeakerSessions { get; set; }
        public virtual ICollection<Attendance> AttendanceList { get; set; }
    }
}
