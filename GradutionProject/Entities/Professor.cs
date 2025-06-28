using GradutionProject.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Entities
{
    public class Professor : Entity
    {

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        ///Relations
        public int? CollegeId { get; set; }
        public College College { get; set; }
        public List<Lecture> Lectures { get; set; }
        public Cours Cours { get; set; }
    }
}
