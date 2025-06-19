using GradutionProject.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Entities
{
    public class Complaint : Entity
    {
       
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
