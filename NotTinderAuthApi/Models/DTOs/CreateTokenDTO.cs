using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTOs;

public class CreateTokenDTO
{
    [Required] public string Name { get; set; }

    [Required] public string DeviceId { get; set; }
}