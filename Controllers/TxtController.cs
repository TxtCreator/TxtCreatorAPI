using Microsoft.AspNetCore.Mvc;
using TxtCreatorAPI.Models;
using TxtCreatorAPI.Services;

namespace TxtCreatorAPI.Controllers;
[Route("[controller]")]
[ApiController]
public class TxtController : ControllerBase
{
    private readonly ITxtService _txtService;

    public TxtController(ITxtService txtService)
    {
        _txtService = txtService;
    }
    
    [HttpPost("download")]
    public IActionResult DownloadTxt([FromBody]TxtModel txtModel)
    {
        Console.WriteLine();
        if (txtModel.Textures.Count == 0)
        {
             return NoContent();
        }
        try
        {
            var txt = _txtService.GetTxt(txtModel);
            return txt;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            return NoContent();
        }
      
    }
    
    [HttpGet("categories/{version}")]
    public ActionResult<List<CategoryModel>> GetAllCategories([FromRoute]string version)
    {
        return _txtService.GetCategories(version);
    }
}
