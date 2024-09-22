using System.ComponentModel.DataAnnotations;

namespace FormDemo.Models
{
    public class StudentModel
    {
        public int StudentId { get; set; }
        [Required (ErrorMessage ="StudentName is Not Empty")]

        public string StudentName { get; set; }
        [Required]
        public string EnrollmentNo { get; set; }
        [Required]
        public string Password { get; set; }
        public int RollNo { get; set; }
        public int CurrentSemester { get; set; }
        [Required(ErrorMessage ="The Email is not empty ")]
        [EmailAddress]
        [Display(Name ="Institute Email")]
        public string EmailInstitute { get; set; }
        public string EmailPersonal { get; set; }
        [Required(ErrorMessage = "Phone Number is Not Empty")]
        [Phone]
        [Range(1,10)]
        [MinLength(6)]
        public string ContactNo { get; set; }
        public int CastID { get; set; }
        public int CityID { get; set; }
        public string Remarks { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }


    }
}
