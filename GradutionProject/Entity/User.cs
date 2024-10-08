using System.ComponentModel.DataAnnotations;

namespace GradutionProject.Entity;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsAdmin { get; set; }
    public string Claim {  get; set; }
    public string PhoneNumber { get; internal set; }
}
