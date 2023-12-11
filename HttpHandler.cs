using Octokit;
using System.IO.Compression;
using FileMode = System.IO.FileMode;

namespace Lethal_Company_Mod_Manager;

internal class HttpHandler
{
    private static readonly HttpClient httpClient = new();

    private const string RepoOwner = "Dev-Nitro";
    private const string RepoName = "Lethal-Company-Mods";

    public static async Task<string> GetStringAsync(string url)
    {
        HttpResponseMessage response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task HttpDownloadFileAsync(string address, string path)
    {
        try
        {
            using var response = await httpClient.GetAsync(address, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            using var contentStream = await response.Content.ReadAsStreamAsync();
            var buffer = new byte[4096];
            int read;

            while ((read = await contentStream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read));
            }
        }
        catch (HttpRequestException ex)
        {
            await ErrorHandler.HandleError(ex, "Error During Init Download");
        }
    }

    public static async Task HttpDownloadFileLogAsync(string endpoint, string destination, string fileName)
    {
        try
        {
            using var response = await httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            long? totalLength = response.Content.Headers.ContentLength;

            using var fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            using var contentStream = await response.Content.ReadAsStreamAsync();
            var totalRead = 0L;
            var buffer = new byte[4096];
            int read;

            while ((read = await contentStream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read));
                totalRead += read;
                if (totalLength.HasValue)
                {
                    double percentage = (double)totalRead / totalLength.Value * 100;
                    Program.PrintColoredMessage("\r[Mod Manager] ", $"Downloading {fileName} | {percentage:F2}%     ", ConsoleColor.Blue, ConsoleColor.White);
                }
            }
        }
        catch (HttpRequestException ex) { await ErrorHandler.HandleError(ex, "Error During Download 1"); }
    }

    public static async Task DownloadFilesFromRepo(string destinationFolder, string RepoPath)
    {
        var client = new GitHubClient(new ProductHeaderValue("Lethal-Company-Mod-Manager"));

        try
        {
            var contents = await client.Repository.Content.GetAllContents(RepoOwner, RepoName, RepoPath);

            foreach (var content in contents)
            {
                if (content.Type == ContentType.File)
                {
                    var filePath = Path.Combine(destinationFolder, content.Name);

                    if (!File.Exists(filePath))
                    {
                        await HttpDownloadFileLogAsync(content.DownloadUrl, filePath, content.Name);

                        if (Path.GetExtension(content.Name).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            string directoryName = Path.Combine(destinationFolder, Path.GetFileNameWithoutExtension(content.Name));

                            if (!Directory.Exists(directoryName))
                            {
                                ExtractZipFile(filePath, destinationFolder);
                                File.Delete(filePath);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex) { await ErrorHandler.HandleError(ex, "Error During Download 2"); }
    }
    private static void ExtractZipFile(string zipFilePath, string extractToFolder)
    {
        Program.PrintColoredMessage("\r[Mod Manager] ", $"Extracting {Path.GetFileName(zipFilePath)}", ConsoleColor.Blue, ConsoleColor.White);
        ZipFile.ExtractToDirectory(zipFilePath, extractToFolder);
    }
}
