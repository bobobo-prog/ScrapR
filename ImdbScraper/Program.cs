using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace ImdbScraper
{
    class Program
    {
        
        static List<string> urllist = new List<string>();

        string aurl = "https://www.imdb.com/title/tt0111161/reviews/_ajax?ref_=undefined&paginationKey=";
        public async void GetHtml(string u)
        {
            

            var url = u;

            

            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);


            var nexturl = htmlDocument.DocumentNode.Descendants("div")
                  .Where(node => node.GetAttributeValue("class", "").Equals("text show-more__control")).ToList();



            foreach (var item in nexturl)
            {
                Console.WriteLine(item.InnerText.ToString());

            }

            }
        
        public async void GetUrls(string u)
        {
            var init_url = u;

            var httpCli = new HttpClient();

            var html = await httpCli.GetStringAsync(init_url);

            var htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(html);

            


            var nxt_url = htmldoc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("load-more-data")).ToList();


            
            if (nxt_url.Count!=0)
            {
                foreach (var item in nxt_url)
                {
                    string url = item.GetAttributeValue("data-key", "").ToString();


                    urllist.Add(url);
                    string succ_url = aurl + url;
                    Console.WriteLine(succ_url);
                    GetUrls(succ_url);
                    Console.WriteLine("Scraping...");

                }
            }
            else if(nxt_url.Count==0)
            {
                Console.WriteLine("idk whats happening");
                ScrapenSave();


            }


        }


        public async void ScrapenSave()
        {
            var url = urllist;
            int rev_count = 1;
            Console.WriteLine("Urls: "+urllist.Count.ToString());
            var records = new List<dynamic>();

            foreach (var stuff in url)
            {

                 string fin_url = aurl + stuff;
                var httpClient = new HttpClient();

                var html = await httpClient.GetStringAsync(fin_url);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);



                // var load = htmlDocument.DocumentNode.Descendants("button")
                //       .Where(node => node.GetAttributeValue("id", "").Contains("load-more-trigger")).ToList();


                var Review = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "notfound").Equals("content")).ToList();


                foreach (var item in Review)
                {
                    Console.WriteLine("Count: " + rev_count);
                    //Console.WriteLine(item.Descendants("div").FirstOrDefault().InnerText.Trim().ToString());
                    string review = item.Descendants("div").FirstOrDefault().InnerText.Trim().ToString();
                    // Console.WriteLine(review);
                    dynamic record = new ExpandoObject();

                    record.Id = rev_count;
                    record.Review = review;
                    records.Add(record);
                    rev_count += 1;
                }

                
            }





            using (var writer = new StreamWriter(@"D:\Games\bulk3.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {


                csv.WriteRecords(records);

                Console.WriteLine("wriiten");

            }








        }





        static void Main(string[] args)
        {

            Program p = new Program();
            string url2 = "https://www.imdb.com/title/tt0111161/reviews?ref_=tt_ql_3";
            p.GetUrls(url2);
            // p.GetHtml(url);
            Console.WriteLine("Done!");
            
            Console.WriteLine("Saving to Excel");
            Console.ReadLine();
        }
    }
}


