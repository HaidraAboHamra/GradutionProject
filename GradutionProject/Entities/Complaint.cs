using GradutionProject.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradutionProject.Entities
{
    public class Complaint : Entity
    {
       
        public string Description { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }
    }
}
