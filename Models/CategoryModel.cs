namespace TxtCreatorAPI.Models;

public class CategoryModel
{
    public string Name { get; set; }
    public string MinecraftPath { get; set; }
    public List<SubCategoryModel> SubCategories {get; set;}
}