using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Entities
{
    public class GradeOfStudent
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int CoursesId { get; set; }
        public Student Student { get; set; }
        public Cours Cours { get; set; }
    }
}
