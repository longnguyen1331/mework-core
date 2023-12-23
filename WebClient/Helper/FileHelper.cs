using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using WebClient.Exceptions;

namespace WebClient.Helper
{
    public static class FileHelper
    {

        public static async Task<byte[]> GetBytesOfExcelFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new NotFoundException("Not Found File");
            }
            
            FileInfo existingFile = new FileInfo(path);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                return await package.GetAsByteArrayAsync();
            }
            
        }

    }
}