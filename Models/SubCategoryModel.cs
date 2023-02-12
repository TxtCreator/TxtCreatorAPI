namespace TxtCreatorAPI.Models;

public class SubCategoryModel
{
    public string Name { get; set; }
    public List<string> Textures { get; set; } = new();
}