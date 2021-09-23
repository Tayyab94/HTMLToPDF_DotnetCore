using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RoundTheCode.Pdf.Models;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RoundTheCode.Pdf.Controllers
{
    [Route("pdf")]
    public class PdfController : Controller
    {
        protected readonly ICompositeViewEngine _compositeViewEngine;

        public PdfController(ICompositeViewEngine compositeViewEngine)
        {
            _compositeViewEngine = compositeViewEngine;
        }

        [Route("website")]
        public async Task<IActionResult> WebsiteAsync()
        {
            var mobileView = new HtmlToPdf();
            mobileView.Options.WebPageWidth = 480;

            var tabletView = new HtmlToPdf();
            tabletView.Options.WebPageWidth = 1024;

            var fullView = new HtmlToPdf();
            fullView.Options.WebPageWidth = 1920;

            var pdf = mobileView.ConvertUrl("https://www.roundthecode.com/");
            pdf.Append(tabletView.ConvertUrl("https://www.roundthecode.com/"));
            pdf.Append(fullView.ConvertUrl("https://www.roundthecode.com/"));

            var pdfBytes = pdf.Save();

            using (var streamWriter = new StreamWriter(@"RoundTheCode.pdf"))
            {
                await streamWriter.BaseStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
            }

            return File(pdfBytes, "application/pdf");
        }

        [Route("invoice")]
        public async Task<IActionResult> InvoiceAsync()
        {
            List<Emp> dat = new List<Emp>()
            {
                new Emp()
                {
                     Description="THis asdada",
                     Price=34
                },
                new Emp()
                {
                     Description="THis is sseconas dasdjahdsja s",
                     Price=44
                },
                new Emp()
                {
                     Description="THis ajskda sjdha sda sda ",
                     Price=22
                },
            };
            using (var stringWriter = new StringWriter())
            {
                //var viewResult = _compositeViewEngine.FindView(ControllerContext, "_Invoice", false);


                var viewResult = _compositeViewEngine.FindView(ControllerContext, "_Invoice", false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"'Views/Pdf/_Invoice.cshtml' does not match any available view");
                }

                //var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());


                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
{
                    Model = dat
                };
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewDictionary,
                    TempData,
                    stringWriter,

                    new HtmlHelperOptions()
                );



                await viewResult.View.RenderAsync(viewContext);

                var htmlToPdf = new HtmlToPdf(1000, 1414);
                htmlToPdf.Options.DrawBackground = true;

                var pdf = htmlToPdf.ConvertHtmlString(stringWriter.ToString());
                var pdfBytes = pdf.Save();

                //using (var streamWriter = new StreamWriter(@"Invoice.pdf"))
                //{
                //    await streamWriter.BaseStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                //}

                return File(pdfBytes, "application/pdf");
            }
        }


     

    }
}
