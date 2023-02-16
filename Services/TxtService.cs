using System.IO.Compression;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TxtCreatorAPI.Models;

namespace TxtCreatorAPI.Services;

public class TxtService : ITxtService
{
    private readonly IWebHostEnvironment _environment;

    public TxtService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public FileContentResult GetTxt(TxtModel txtModel)
    {
        var txtDirectory = $"./{txtModel.Name}-{DateTime.Now.Ticks}-{Path.GetRandomFileName()}";
        CloneDirectory($"{_environment.WebRootPath}/assets/txt{txtModel.Version}", txtDirectory);
        var textures = new Dictionary<FileInfo, string>();
        foreach (var texture in txtModel.Textures)
        {
            var textureFile = new FileInfo($"{_environment.WebRootPath}/{texture.Key}");
            if (textureFile.Name == "plain.png")
            {
                foreach (var filename in Directory.GetFiles(textureFile.DirectoryName))
                {
                    if (!filename.Contains("plain"))
                    {
                        textures.Add(new FileInfo(filename), texture.Value);
                    }
                }
            }
            else
            {
                textures.Add(textureFile, texture.Value);
            }
        }

        var bytes = CloneTextures(textures, txtDirectory);
        if (bytes == null) throw new Exception();
        return new FileContentResult(bytes, MediaTypeNames.Application.Octet);
    }

    public List<CategoryModel> GetCategories(string version)
    {
        var categories = new List<CategoryModel>();
        foreach (var directory in Directory.GetDirectories($"{_environment.WebRootPath}/assets/{version}").Select(directory => new DirectoryInfo(directory)))
        {
            var minecraftPathFile = Directory.GetFiles(directory.FullName).FirstOrDefault();
            if (minecraftPathFile == null)
            {
                continue;
            }
            var minecraftPath = new FileInfo(minecraftPathFile).Name.Replace("-", "/");
            categories.Add(new CategoryModel()
            {
                Name = directory.Name,
                SubCategories = GetSubCategories($"{_environment.WebRootPath}/assets/{version}/{directory.Name}"),
                MinecraftPath = minecraftPath
            });
        }

        return categories;
    }

    private static List<SubCategoryModel> GetSubCategories(string directoryPath)
    {
        var subCategories = new List<SubCategoryModel>();
        foreach (var subDirectory in Directory.GetDirectories(directoryPath).Select(subDirectory => new DirectoryInfo(subDirectory)))
        {
            var textures = Directory.GetFiles(subDirectory.FullName).Select(file =>
            {
                var fileInfo = new FileInfo(file);
                var path = $"{fileInfo.Directory.Parent.Parent.Parent.Name}/{fileInfo.Directory.Parent.Parent.Name}/{fileInfo.Directory.Parent.Name}/{fileInfo.Directory.Name}/{fileInfo.Name}";
                return path;
            }).ToList();
            if (subDirectory.GetDirectories().Length != 0)
            {
                foreach (var dir in subDirectory.GetDirectories())
                {
                    var fileInfo = dir.GetFiles().FirstOrDefault(file => file.Name == "plain.png");
                    if (fileInfo == null) continue;
                    var path = $"{fileInfo.Directory.Parent.Parent.Parent.Parent.Name}/{fileInfo.Directory.Parent.Parent.Parent.Name}/{fileInfo.Directory.Parent.Parent.Name}/{fileInfo.Directory.Parent.Name}/{fileInfo.Directory.Name}/{fileInfo.Name}";
                    textures.Add(path);
                }
            }
            subCategories.Add(new SubCategoryModel()
            {
                Name = subDirectory.Name,
                Textures = textures
            });
        }

        return subCategories;
    }

    private static byte[]? CloneTextures(Dictionary<FileInfo, string> textures, string txtDirectory)
    {
        byte[] bytes;
        try
        {
            foreach (var texture in textures)
            {
                texture.Key.CopyTo(
                    $"./{txtDirectory}/assets/minecraft/{texture.Value}/{texture.Key.Name.Replace("9", "")}",
                    true);
            }
            ZipFile.CreateFromDirectory($"./{txtDirectory}", $"./{txtDirectory}.zip");
            bytes = File.ReadAllBytes($"./{txtDirectory}.zip");
        }
        finally
        {
            Directory.Delete($"./{txtDirectory}", true);
            File.Delete($"./{txtDirectory}.zip");
        }
        return bytes;
    }
    
    private static void CloneDirectory(string root, string dest)
    {
        foreach (var directory in Directory.GetDirectories(root))
        {
            var newDirectory = Path.Combine(dest, Path.GetFileName(directory));
            Directory.CreateDirectory(newDirectory);
            CloneDirectory(directory, newDirectory);
        }

        foreach (var file in Directory.GetFiles(root))
        {
            File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
        }
    }
}

public interface ITxtService
{
    FileContentResult GetTxt(TxtModel txtModel);
    List<CategoryModel> GetCategories(string version);
}