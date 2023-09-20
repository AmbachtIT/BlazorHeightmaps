using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Blazor.Services
{
    public interface IDownloadGeneratedFileService
    {

        Task SaveAs(string filename, byte[] data, string mimeType);

    }
}
