namespace TCSA.KnowTheCity.Core.Models.DTOs;

public class ServiceResponse
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; } = string.Empty;
}

public sealed class ServiceResponse<T> : ServiceResponse
{
    public T? Data { get; set; }
}