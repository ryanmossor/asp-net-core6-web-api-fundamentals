using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class City
    {
        [Key] // still useful to explicity set primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // "Id" will be an identity column and will generate a new ID when a city is added
        public int Id { get; set; } // properties containing "Id" automatically get set as primary key

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<PointOfInterest> PointsOfInterest { get; set; } = new List<PointOfInterest>();

        public City(string name)
        {
            Name = name; // specifies City should always have a name
        }
    }
}
