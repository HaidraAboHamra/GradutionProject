using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Entities
{
    public class Professsor
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        ///Relations
        public int? CollegeId { get; set; }
        public College College { get; set; }
        public List<Lecture> Lectures { get; set; }
    }
}
