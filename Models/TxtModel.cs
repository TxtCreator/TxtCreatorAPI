namespace TxtCreatorAPI.Models;

public class TxtModel
{
    public string Name { get; set; }
    public string Version { get; set; }
    public Dictionary<string, string> Textures { get; set; }
}