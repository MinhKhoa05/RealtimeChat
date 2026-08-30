using System.ComponentModel.DataAnnotations;

namespace RealtimeChat.BLL.Options;

public class AppOptions
{
    [Required]
    public string BaseUrl { get; set; } = string.Empty;
}
