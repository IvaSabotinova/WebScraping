using HtmlAgilityPack;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Text;

namespace WebScraping_CSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string pdfPath = "../../../../Test task - Web Scraping Specialist.pdf";

            if (!File.Exists(pdfPath))
            {
                Console.WriteLine("PDF file not found.");
                return;
            }

            try
            {
                StringBuilder sb = new StringBuilder();          

               // Create a PdfReader object to read the PDF file
                using (PdfReader pdfReader = new PdfReader(pdfPath))
                {
                    // Create a PdfDocument document in reading mode
                    using PdfDocument pdfDocument = new PdfDocument(pdfReader);

                    //Get the text of the pdf file page by page
                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {                        
                        sb.Append(PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i)).TrimEnd());
                    }
                }

                //Method for extracting needed products'data
                List<Product> products = ExtractData(sb.ToString());

                Console.WriteLine(JsonConvert.SerializeObject(products, Formatting.Indented));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }

        /// <summary>
        /// Get a list of all needed product data.
        /// </summary>
        /// <param name="pdfAsText"></param>
        /// <returns></returns>
        private static List<Product> ExtractData(string pdfAsText)
        {
            List<Product> products = new List<Product>();

            // Start parsing HTML using HtmlAgilityPack
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pdfAsText);

            // Get the 3 main divs all with class="item"
            HtmlNodeCollection divs = htmlDocument.DocumentNode.SelectNodes("div[@class='item']");

            //Iterating through all the divs as HtmlNodes to get the needed products' info
            foreach (HtmlNode node in divs)
            {
                //Get encoded product name from HTML
                string? encodedProductName = node.Descendants("img")
                    .Select(x => x.Attributes["alt"].Value)
                    .FirstOrDefault();

                //Decoding product name
                string productName = WebUtility.HtmlDecode(encodedProductName) ?? string.Empty;

                //Get product price from HTML
                HtmlNode? priceNode = node.Descendants("span")
                    .Where(x => x.Attributes["style"]?.Value.Contains("display: none") ?? false)
                    .FirstOrDefault();

                //Setting price to default in case priceNode above is null
                decimal price = default;

                if (priceNode != null)
                {
                    //If parsing to decimal is successful it will return the needed value otherwise the default value
                    bool priceAsBoolean = decimal.TryParse(priceNode.InnerText.Trim().Remove(0, 1), out decimal currPrice);

                    price = currPrice;
                }

                //Get rating from HTML
                string ratingAsString = node.GetAttributeValue("rating", string.Empty);

                //If parsing to decimal is successful it will return the needed value otherwise the default value
                bool ratingAsBoolean = decimal.TryParse(ratingAsString, out decimal rating);
                                
                NormalizeRating(ref rating);

                //Adding all data for products in the list of products
                products.Add(new Product()
                {
                    Name = productName,
                    Price = price.ToString(),
                    Rating = rating.ToString(),                      
                });
            }

            return products;
        }

        /// <summary>
        /// Normalizing the rating value to be out of 5 if it's not.
        /// </summary>
        /// <param name="rating"></param>
        private static void NormalizeRating(ref decimal rating)
        {
            if (rating > 5)
            {
                decimal divider = 2;
                while ((rating / divider) > 5)
                {
                    divider++;
                }
                rating /= divider;
            }
        }

    }
}