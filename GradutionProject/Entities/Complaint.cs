using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Entities
{
    public class Complaint
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
