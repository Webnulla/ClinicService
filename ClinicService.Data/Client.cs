using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.Data;

[Table("Clients")]
public class Client
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Column]
    [StringLength(50)]
    public string? Document { get; set; }
    
    [Column]
    [StringLength(255)]
    public string? Surname { get; set; }
    
    [Column]
    [StringLength(255)]
    public string? FirstName { get; set; }
    
    [Column]
    [StringLength(255)]
    public string? Patronymic { get; set; }
    
    //у одного клиента может быть много животных и консультаций
    [InverseProperty(nameof(Pet.Client))] 
    public ICollection<Pet> Pets { get; set; } = new HashSet<Pet>();
    
    [InverseProperty(nameof(Consultation.Client))] 
    public ICollection<Consultation> Collections { get; set; } = new HashSet<Consultation>();
}