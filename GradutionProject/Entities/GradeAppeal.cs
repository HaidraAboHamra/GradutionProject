namespace GradutionProject.Entities
{
    public class GradeAppeal
    {
        public int Id { get; set; }

        public int GradeOfStudentId { get; set; }
        public GradeOfStudent GradeOfStudent { get; set; }

        public int StudentId { get; set; } 
        public int CourseId { get; set; }

        public bool? AppealOnLabs { get; set; } 
        public bool? AppealOnStudiom { get; set; }

        public decimal? RequestedDegreeOfLabs { get; set; } 
        public decimal? RequestedDegreeOfStudiom { get; set; } 

        public string Reason { get; set; } 

        public string Status { get; set; } = "Pending"; 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ResolvedAt { get; set; }

        public int? ResolvedByProfessorId { get; set; }
    }

}
