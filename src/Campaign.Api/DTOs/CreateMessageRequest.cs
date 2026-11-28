using System.ComponentModel.DataAnnotations;

namespace Campaign.Api.DTOs;

public sealed class CreateMessageRequest
{
    [Required]
    public string Content { get; init; } = string.Empty;
}
