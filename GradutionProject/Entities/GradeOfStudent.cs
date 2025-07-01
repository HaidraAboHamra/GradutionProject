using GradutionProject.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GradutionProject.Entities;

public class GradeOfStudent : Entity
{
    public decimal DegreeOfStudiom { get; set; }
    public decimal DegreeOfLabs { get; set; }

    public int StudentId { get; set; }
    public int CoursesId { get; set; }

    [ForeignKey(nameof(StudentId))]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey(nameof(CoursesId))]
    public virtual Cours Cours { get; set; } = null!;
}
