using System.ComponentModel.DataAnnotations;

namespace C___ASP.NET_Core_.Entities;


public class Game
{
    // properties
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public required string Name { get; set; } // c# has a required keyword that tells the compiler that the field isnt nullable. In other words, when we create an instance of the class a name must be specified.
    
    [Required]
    [StringLength(20)]
    public required string Genre { get; set; }
    
    [Range(1, 100)]
    public decimal Price { get; set; } // for money always use type decimal not double
    public DateTime ReleaseDate { get; set; }
    
    [Url]
    [StringLength(100)]
    public required string ImageUri { get; set; }
}