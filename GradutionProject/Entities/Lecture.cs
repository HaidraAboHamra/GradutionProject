using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradutionProject.Entities;

public class Lecture
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int NumberOfLectures { get; set; }
    public byte[] Pdf { get; set; }

    ///Relations
    public int? CollegeId { get; set; }
    [ForeignKey("CollegeId")]
    public College College { get; set; }
    public int? ProfessorId { get; set; }
    [ForeignKey("ProfessorId")]
    public Professor Professsor { get; set; }

}
