using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Google.Protobuf.WellKnownTypes;

namespace ClinicService.Data;

[Table("Pets")]
public class Pet
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Client))] 
    public int ClientId { get; set; }
    
    [Column]
    [StringLength(20)]
    public string Name { get; set; }
    
    [Column]
    public Timestamp Birthday { get; set; }
    
    public Client Client { get; set; }

    //у одного животного может быть много консультаций
    [InverseProperty(nameof(Consultation.Pet))]
    public ICollection<Consultation> Consultations { get; set; } = new HashSet<Consultation>();
}