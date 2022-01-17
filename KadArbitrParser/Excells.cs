using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KadArbitrParser
{
    internal class Excells
    {
        void work()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo($@"{Directory.GetCurrentDirectory()}\test.xlsx");
        }
    }
}
