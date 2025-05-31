using GradutionProject.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Entities
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public Gender? Gender { get; set; }
        public string? College { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birth { get; set; }
        public DateTime? CertificateDate { get; set; }
        public string? NationalId { get; set; }

        public byte[]? CertificateImg { get; set; }
        public byte[]? PersonalPhoto { get; set; }
        public byte[]? Invoice { get; set; }
    }
}
