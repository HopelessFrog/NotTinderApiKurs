namespace AuthApi.Models.DTOs;

public record TokenDto(string AccessToken = "", string RefreshToken = "");