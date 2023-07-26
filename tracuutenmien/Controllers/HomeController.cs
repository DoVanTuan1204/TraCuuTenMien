using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Text.RegularExpressions;



namespace tracuutenmien.Controllers

{
   



    public class HomeController : Controller
    {
       
        // Helper method to extract domain information from the HTML content using regular expressions

        // Helper method to extract value between start and end strings
        private string GetInfoValue(string input, string start, string end)
    {
        Match match = Regex.Match(input, $"{start}(.*?){end}");
        return match.Success ? match.Groups[1].Value.Trim() : "";
    }

        private string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        private readonly HttpClient httpClient;
        public HomeController()
        {
            httpClient = new HttpClient();
        }
        public async Task<ActionResult> Index(string TenMien)
        {
            string apiUrl = "http://49.156.54.103:8088/check";
            string tenmien = TenMien;
            string key = "JpbmNlcyBTdHJl311dsjdsj1144RnRyYWwsIEF1Y2tsYW";

            ViewBag.Test = TenMien;

            string queryString = $"?tenmien={tenmien}&key={key}";

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl + queryString);
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    {
                        string html = apiResponse;

                        // Define the start and end markers as regular expressions
                        string startMarkerPattern = "<!--\\s*Main content\\s*-->";
                        string endMarkerPattern = "<!--\\.\\/Main content-->";

                        // Find the content between the markers using regular expressions
                        Regex startRegex = new Regex(startMarkerPattern);
                        Regex endRegex = new Regex(endMarkerPattern);

                        Match startMatch = startRegex.Match(html);
                        Match endMatch = endRegex.Match(html);

                        if (startMatch.Success && endMatch.Success && endMatch.Index > startMatch.Index)
                        {
                            // Extract the content between the markers
                            int startIndex = startMatch.Index + startMatch.Length;
                            int endIndex = endMatch.Index;
                            string extractedContent = html.Substring(startIndex, endIndex - startIndex);

                            string cleanedContent = RemoveHtmlTags(extractedContent);

                            string data = cleanedContent;
                            // Extract the required information from the HTML content and create the DomainInfo object


                            int domainNameIndex = data.IndexOf("THÔNG TIN TRA CỨU") + "THÔNG TIN TRA CỨU".Length;
                            int domainNameEnd = data.IndexOf("Loại tên miền:");
                            string domainType = data.Substring(domainNameIndex, domainNameEnd - domainNameIndex).Trim();
                            ViewBag.Response = domainType; // Pass the API response to the view 

                            if (domainType == "Tên miền quốc gia .VN")
                            {
                                domainNameIndex = data.IndexOf("Loại tên miền:") + "Loại tên miền:".Length;
                                domainNameEnd = data.IndexOf("Tên chủ thể");
                                string domainName = data.Substring(domainNameIndex, domainNameEnd - domainNameIndex).Trim();
                                ViewBag.Status = domainName; // Pass the API response to the view 

                                domainNameIndex = data.IndexOf("Tên chủ thể đăng ký sử dụng:") + "Tên chủ thể đăng ký sử dụng:".Length;
                                domainNameEnd = data.IndexOf("Nhà đăng ký quản lý:");
                                domainName = data.Substring(domainNameIndex, domainNameEnd - domainNameIndex).Trim();
                                ViewBag.Owner = domainName; // Pass the API response to the view 

                                domainNameIndex = data.IndexOf("Nhà đăng ký quản lý:") + "Nhà đăng ký quản lý:".Length;
                                domainNameEnd = data.IndexOf("Ngày đăng ký:");
                                domainName = data.Substring(domainNameIndex, domainNameEnd - domainNameIndex).Trim();
                                ViewBag.Manager = domainName; // Pass the API response to the view 

                                domainNameIndex = data.IndexOf("Ngày đăng ký:") + "Ngày đăng ký:".Length;
                                domainNameEnd = data.IndexOf("Ngày hết hạn:");
                                domainName = data.Substring(domainNameIndex, domainNameEnd - domainNameIndex).Trim();
                                ViewBag.RegisterDay = domainName; // Pass the API response to the view 

                                domainNameIndex = data.IndexOf("Ngày hết hạn: ") + "Ngày hết hạn: ".Length;
                                domainNameEnd = data.LastIndexOf(data);
                                domainName = data.Substring(domainNameEnd, domainNameIndex - domainNameEnd).Trim();

                                // domainNameIndex = data.LastIndexOf("Ngày hết hạn: ") + "Ngày hết hạn: ".Length;
                                //  domainName = data.Substring(domainNameIndex, 9).Trim();
                                ViewBag.EndDay = domainName;
                            }
                            else
                                if(domainType== "Tên miền quốc tế")
                            {
                                ViewBag.EndDay = data;
                                domainNameIndex = data.IndexOf("Loại tên miền:") + "Loại tên miền:".Length;
                                domainNameEnd = data.IndexOf("Trạng thái:");
                                string domainName = data.Substring(domainNameIndex, domainNameEnd - domainNameIndex).Trim();
                                ViewBag.Status = domainName; // Pass the API response to the view 

                                domainNameIndex = data.IndexOf("Trạng thái:") + "Trạng thái:".Length;
                                domainNameEnd = data.IndexOf("Thông tin chi tiết:");
                                domainName = data.Substring(domainNameIndex, domainNameEnd - domainNameIndex).Trim();
                                ViewBag.Owner = domainName; // Pass the API response to the view 

                            }
                        }
                        else
                        {
                            // Handle the case when markers are not found
                            Console.WriteLine("Start or end markers not found in the HTML content.");
                        }

                        return View();
                    }

                }
                else
                {
                    ViewBag.Response = "API request failed.";
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Response = $"API request failed. Error: {ex.Message}";
            }

            return View();
        }
    }
}