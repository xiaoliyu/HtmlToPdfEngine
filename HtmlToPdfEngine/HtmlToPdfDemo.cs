using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.Xml;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.InteropServices;

namespace HtmlToPdfEngine
{
    public  class HtmlToPdfDemo
    {     
        private readonly IConverter _converter;
        public HtmlToPdfDemo(IConverter conv)
        {
            _converter =conv;
        }
       
        [FunctionName("Convert")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Convert Html To Pdf");

            string html = @"<html>
<body>
<h1>My First Heading</h1>
</body>
</html>
";           
            try
            {
                var pdfDoc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                                ColorMode = ColorMode.Color,
                                Orientation = Orientation.Portrait,
                                PaperSize = PaperKind.Letter,
                            },
                    Objects = {
                                new ObjectSettings() {
                                    PagesCount = true,
                                    HtmlContent =html,
                                    WebSettings = { DefaultEncoding = "utf-8" },
                                    HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                                }
                              }
                };


                var bytes = _converter.Convert(pdfDoc);

                return new FileContentResult(bytes, "application/pdf");
            }
            catch(Exception ex)
            {
                return new ObjectResult(ex) { StatusCode=500 };
            }
            //if (Directory.Exists("/lib/x86_64-linux-gnu/"))
            //{
            //    string[] files = Directory.GetFiles("/lib/x86_64-linux-gnu/", "*.*");
            //    return new ObjectResult(files);
            //}

            //return new ObjectResult("Not found /lib/x86_64-linux-gnu/");
        }
    }
}
