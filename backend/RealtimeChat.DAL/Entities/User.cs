using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealtimeChat.DAL.Entities;

[Table("users")]
public class User
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}