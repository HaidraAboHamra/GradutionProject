using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradutionProject.Entities
{
    public class College
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int YearOfStudy { get; set; }

        //Relations
        public List<Student> Students { get; set; }
        public int? AdminId { get; set; }
        [ForeignKey("AdminId")]
        public Admin Admin { get; set; }
        public List<Professsor> Professsors { get; set; }
    }
}
