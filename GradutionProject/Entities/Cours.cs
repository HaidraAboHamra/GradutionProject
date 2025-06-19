using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradutionProject.Entities
{
    public class Cours
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal DegreeOfLabs { get; set; }
        public decimal DegreeOfStudiom {  get; set; }

        ///Relations
        public int? CollegeId { get; set; }
        [ForeignKey("CollegeId")]
        public College College { get; set; }
        public int? ProfessorId { get; set; }
        public Professor Professsor { get; set; }


    }
}
