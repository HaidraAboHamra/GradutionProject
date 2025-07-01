using GradutionProject.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradutionProject.Entities
{
    public class GradeAppeal : Entity
    {
        public int GradeOfStudentId { get; set; }

        [ForeignKey(nameof(GradeOfStudentId))]
        public virtual GradeOfStudent GradeOfStudent { get; set; } = null!;

        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;
        public int CourseId { get; set; }

        public bool? AppealOnLabs { get; set; }
        public bool? AppealOnStudiom { get; set; }

        public decimal? RequestedDegreeOfLabs { get; set; }
        public decimal? RequestedDegreeOfStudiom { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ResolvedAt { get; set; }

        public int? ResolvedByProfessorId { get; set; }
    }
}
