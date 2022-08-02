using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAppServer.Data;

[Table("Contract")]
public class Contract
{
    [Key]  
    public int UId { get; set; }

    public DateTime? DateC { get; set; }
}