using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace qbService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssamblyController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        string directory = "AppDesktop";

        public AssamblyController(IWebHostEnvironment hostingEnvironment) {
            this._hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("getAssablyVersion")]
        public ContentResult GetAssablyVersion()
        {
            // string ruta = Path.Combine(_hostingEnvironment.WebRootPath, directory, "assambly.txt");
            string ruta = Path.Combine(_hostingEnvironment.WebRootPath, directory, "wpf.exe");
            if (System.IO.File.Exists(ruta))
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(ruta);
                string version = versionInfo.FileVersion;
                //var data = System.IO.File.ReadAllText(ruta);
                return Content(version);
            }
            return Content("");
        }

        [HttpPost]
        [Route("ComparNewFile")]
        public IActionResult ComparNewFile([FromBody] List<FileInformation> fileInformation)
        {
            string ruta = Path.Combine(_hostingEnvironment.WebRootPath, directory);
            List<FileInformation> fileInformationResul = new List<FileInformation>();
            string[] allfiles = Directory.GetFiles(ruta, "*.*", SearchOption.AllDirectories);
            foreach (string item in allfiles)
            {
                if (System.IO.File.Exists(item))
                {
                    var fi1 = new FileInfo(item);
                    if (fi1.Name != "UDQS.deps.json" &&
                        fi1.Name != "UDQS.dll" &&
                        fi1.Name != "UDQS.exe" &&
                        fi1.Name != "UDQS.pdb" &&
                        fi1.Name != "UDQS.runtimeconfig.dev.json" &&
                        fi1.Name != "UDQS.runtimeconfig.json")
                    {
                        fileInformationResul.Add(new FileInformation()
                        {
                            Name = item.Replace(ruta, ""),
                            DateModified = fi1.LastWriteTime,
                            length = fi1.Length
                        });
                    }
                }
            }
            if (fileInformation.Count != 0)
            {
                for (int i = 0; i < fileInformationResul.Count; i++)
                {
                    var item1 = fileInformationResul[i];
                    for (int j = 0; j < fileInformation.Count; j++)
                    {
                        var item2 = fileInformation[j];
                        if (item1.Name == item2.Name && (item1.DateModified <= item2.DateModified || item1.length != item2.length))
                        {
                            fileInformationResul.Remove(item1);
                            fileInformation.Remove(item2);
                            i--;
                            j--;
                            break;
                        }
                    }
                }
            }
            return Ok(fileInformationResul);
        }

        [HttpPost]
        [Route("DonwloadFile")]
        public IActionResult DonwloadFile([FromBody] FileInformation fileInformation)
        {
            string ruta = _hostingEnvironment.WebRootPath+"\\" +directory+ fileInformation.Name;
            if (System.IO.File.Exists(ruta))
            {
                FileInfo a = new FileInfo(ruta);

                string mimeType = "application/unknown";
                string ext = System.IO.Path.GetExtension(ruta).ToLower();
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();

                MemoryStream file = new MemoryStream(System.IO.File.ReadAllBytes(ruta));
                var prueba = File(file, mimeType, a.Name, true);
                return prueba;
            }
            return Ok();
        }
    }

    public class FileInformation 
    {
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public long length { get; set; }
    }

}