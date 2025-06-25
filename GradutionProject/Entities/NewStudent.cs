using GradutionProject.Abstractions;
using GradutionProject.Entities.Enums;

namespace GradutionProject.Entities
{
    public class NewStudent : Entity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public Gender? Gender { get; set; }
        public int YearOfStudy { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birth { get; set; }
        public DateTime? CertificateDate { get; set; }
        public string? NationalId { get; set; }

        public byte[]? CertificateImg { get; set; }
        public byte[]? PersonalPhoto { get; set; }
        public byte[]? Invoice { get; set; }

        ///Relations
        public int? CollegeId { get; set; }
        public College College { get; set; }
    }
}
