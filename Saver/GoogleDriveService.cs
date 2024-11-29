using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saver
{

    public class GoogleDriveService
    {
        private readonly DriveService _driveService;
        private const string CredentialsFileName = "credentials.json";
        private const string TokenFileName = "token.json";

        public GoogleDriveService()
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var credsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");
            if (!File.Exists(credsPath))
            {
                throw new FileNotFoundException($"Google API credentials file not found at {credsPath}");
            }
            var tokenPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token.json");
            using var stream = new FileStream(credsPath, FileMode.Open, FileAccess.Read);
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                new[] { DriveService.Scope.DriveFile },
                "user",
                CancellationToken.None,
                new FileDataStore(tokenPath, true)
            ).Result;
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "MauiApp24"
            });
        }

        public async Task UploadAsync(string fileName, string content, string mimeType)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
                await File.WriteAllTextAsync(tempFilePath, content);
                await UploadFileAsync(tempFilePath, mimeType);
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
            else
            {
                throw new ArgumentException("Content cannot be null or empty", nameof(content));
            }
        }

        private async Task UploadFileAsync(string filePath, string mimeType)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = Path.GetFileName(filePath),
            };

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var request = _driveService.Files.Create(fileMetadata, fileStream, mimeType);
            request.Fields = "id";
            var result = await request.UploadAsync();

            if (result.Status != Google.Apis.Upload.UploadStatus.Completed)
            {
                throw new Exception($"Failed to upload file: {result.Exception?.Message}");
            }
        }
    }

}
