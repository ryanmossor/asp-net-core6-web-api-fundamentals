using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        // A relationship will be created when a navigation property is discovered on the PointOfInterest type.
        // A property is considered a navigation property if the type it points to can't be mapped as a scalar type by the DB provider.
        [ForeignKey("CityId")] // not necessary, but again useful to be explicit
        public City? City { get; set; } // by convention, discovered relationships will always start with primary key of the principal entity (Id of City in this case)
        public int CityId { get; set; }
        
        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}
