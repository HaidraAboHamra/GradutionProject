using GradutionProject.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradutionProject.Entities
{
    public class News : Entity
    {
        public int AdminId { get; set; }
        public string Description { get; set; }

        [ForeignKey("AdminId")]
        public Admin Admin { get; set; }
    }
}
