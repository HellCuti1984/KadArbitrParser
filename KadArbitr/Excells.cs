using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace KadArbitrParser
{
    internal class Excells
    {
        static public async void Save(List<KadModel> KadModel, string fileName = "kad_arbitr_pars")
        {
            string dirPath = $@"{Directory.GetCurrentDirectory()}\data\";
            Directory.CreateDirectory(dirPath);

            string path = $@"{dirPath}{fileName}.xlsx";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(path);

            await SaveExcelFile(KadModel, file);
        }

        private static async Task SaveExcelFile(List<KadModel> kads, FileInfo file)
        {
            DeleteIfExists(file);

            using var package = new ExcelPackage(file);

            var ws = package.Workbook.Worksheets.Add("kad_arbitr");

            var range = ws.Cells["A1"].LoadFromCollection(kads, true);
            range.AutoFitColumns();

            ws.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Row(1).Style.Font.Bold = true;
            ws.Column(3).Width = 20;

            await package.SaveAsync();
        }

        private static void DeleteIfExists(FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
        }
    }
}
