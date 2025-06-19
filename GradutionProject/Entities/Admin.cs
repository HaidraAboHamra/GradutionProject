using GradutionProject.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Entities
{
    public class Admin : Entity
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }

        ///Relations
        public College? College { get; set; }
    }
}
