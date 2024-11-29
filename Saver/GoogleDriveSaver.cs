using Parsers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace Saver
{
    public class GoogleDriveSaver
    {
        private readonly GoogleDriveService _googleDriveService;

        public GoogleDriveSaver()
        {
            _googleDriveService = new GoogleDriveService();
        }

        public async Task SaveToGoogleDriveAsync(List<Subject> subjects, string selectedFormat)
        {
            var saver = SaverFactory.GetSaver(selectedFormat);
            string fileContent = saver.GenerateContent(subjects);
            string fileName = selectedFormat == "XML" ? "schedule.xml" : "schedule.html";
            string mimeType = selectedFormat == "XML" ? "application/xml" : "text/html";
            await _googleDriveService.UploadAsync(fileName, fileContent, mimeType);
        }
    }


}
