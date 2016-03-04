using System.ComponentModel.DataAnnotations;

namespace Presentation.Web.Models.Student 
{
    public class GpaStudentViewModel 
    {
        public int Id { get; set; }
        [Display(Name="Student Name")]
        public string Name { get; set; }
        public float AverageGrade { get; set; }
    }
}