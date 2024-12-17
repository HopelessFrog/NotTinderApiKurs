using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace StartupsApi.Models.Requests;

public class CreateOrUpdateStartupRequest
{
    [ModelBinder(BinderType = typeof(JsonModelBinder))]
    public StartupData Data { get; set; }

    public IFormFile Icon { get; set; }
    public List<IFormFile> Images { get; set; }
}