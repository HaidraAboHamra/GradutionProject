using GradutionProject.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradutionProject.Entities;

public class GradeOfStudent: Entity
{

    public decimal DegreeOfStudiom { get; set; }
    public decimal DegreeOfLabs { get; set; } 
    public int StudentId { get; set; }
    public int CoursesId { get; set; }
    [ForeignKey("StudentId")]
    public Student Student { get; set; }
    [ForeignKey("CoursesId")]
    public Cours Cours { get; set; }
}
