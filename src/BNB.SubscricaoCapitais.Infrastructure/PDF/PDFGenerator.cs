using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace BNB.ProjetoReferencia.Infrastructure.PDF;

[Service(ServiceLifetime.Scoped, typeof(IPDFGenerator))]
public class PDFGenerator : IPDFGenerator
{
    public byte[] Generate(string html, CancellationToken cancellationToken)
    {
        using var pdfMemoryStream = new MemoryStream();
        PdfWriter pdfWriter = new(pdfMemoryStream);
        PdfDocument pdfDocument = new(pdfWriter);
        var byteArrayHtml = Encoding.UTF8.GetBytes(html);

        using (var htmlMemoryStream = new MemoryStream(byteArrayHtml))
        {
            HtmlConverter.ConvertToPdf(htmlMemoryStream, pdfDocument);
        }

        pdfDocument.Close();
        var pdfStream = pdfMemoryStream.ToArray();

        return pdfStream;
    }
}
