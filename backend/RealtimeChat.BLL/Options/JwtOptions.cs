using System.ComponentModel.DataAnnotations;

namespace RealtimeChat.BLL.Options;

public class JwtOptions
{
    [Required]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    public int ExpireMinutes { get; set; } = 30;
}
