using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using PictureThief.Interfaces;

namespace PictureThief.Impl
{
    public class PTReiss : PictureProcessorBase
    {
        public PTReiss() :
            base("Reiss")
        {

        }

        protected override bool PareparePicInfo(PageInfo pi, string htmlContent)
        {

            //htmlContent = htmlContent.ToLower();
            htmlContent = htmlContent.Replace(System.Environment.NewLine, "");
            htmlContent = htmlContent.Replace("&euro;", "EURO ");

            string[] patterns = new string[] { @"<li class=""category_product(((?!</li>)[\s\S])*)<img class=""image_1 showing""(((?!src)[\s\S])*)src=""(?<URL1>((?!"")(?!>)[\s\S])*)""(([\s])*)" + 
            @"data-src=""(?<URL1DS>((?!"")(?!>)[\s\S])*)""(((?!</li>)[\s\S])*)" + 
            @"<img class=""image_2""(((?!src)[\s\S])*)src=""(?<URL2>((?!"")(?!>)[\s\S])*)""(([\s])*)" + 
            @"data-src=""(?<URL2DS>((?!"")(?!>)[\s\S])*)""(((?!</li>)[\s\S])*)" + 
            @"<h2 class=""delta cat_product_title"">(((?!>)[\s\S])*)>(?<ProductTitle>((?!>)[\s\S])*)([\s])*</a>(((?!</li>)[\s\S])*)" + 
            @"<a class=""product_short_desc(((?!>)[\s\S])*)>(?<ProductDesc>((?!>)[\s\S])*)</a>(((?!</li>)[\s\S])*)<a class=""product_colour_desc""(((?!>)[\s\S])*)>(?<ProductColorDesc>((?!>)[\s\S])*)</a>(((?!</li>)[\s\S])*)" + 
            @"<span class=""was_price""(((?!>)[\s\S])*)>(?<WasPrice>((?!>)[\s\S])*)</span>(((?!</li>)[\s\S])*)<span class=""now_price""(((?!>)[\s\S])*)>(?<NowPrice>((?!>)[\s\S])*)</span>(((?!</li>)[\s\S])*)</li>"};


            //<a href=""(?<url>((?!<a href)[\s\S])*)"">(?<titleContent>((?!<a href)[\s\S])*)</a>[\s]*<a href=""(?<url_2>((?!<a href)[\s\S])*)"">(?<titleContent_2>((?!<a href)[\s\S])*)</a>[\s]*</dd>" };


            List<Match> collections = new List<Match>();

            foreach (string pattern in patterns)
            {
                MatchCollection collectionsTemp = Regex.Matches(htmlContent, pattern);

                if (collectionsTemp != null && collectionsTemp.Count > 0)
                {
                    Match[] matches = new Match[collectionsTemp.Count];
                    collectionsTemp.CopyTo(matches, 0);
                    collections.AddRange(matches);
                }
            }


            int count = 0;
            foreach (Match match in collections)
            {
                PicInfoProcessor pip = new PicInfoProcessor(pi, SaveToDirectory);

                pip.ID = count++;
                string ds1 = match.Groups["URL1DS"].Value;
                string ds2 = match.Groups["URL2DS"].Value;

                if (string.IsNullOrEmpty(ds1))
                {
                    pip.Pic1Url = match.Groups["URL1"].Value;
                }
                else
                {
                    pip.Pic1Url = ds1;
                }

                if (string.IsNullOrEmpty(ds2))
                {
                    pip.Pic2Url = match.Groups["URL2"].Value;
                }
                else
                {
                    pip.Pic2Url = ds2;
                }

                pip.PicDetailPair.Add(new KeyValuePair<string, string>("Product Title", TrimEndStart(match.Groups["ProductTitle"].Value)));
                pip.PicDetailPair.Add(new KeyValuePair<string, string>("Short Description", TrimEndStart(match.Groups["ProductDesc"].Value)));
                pip.PicDetailPair.Add(new KeyValuePair<string, string>("Colour Description", TrimEndStart(match.Groups["ProductColorDesc"].Value)));
                pip.PicDetailPair.Add(new KeyValuePair<string, string>("Was Price", TrimEndStart(match.Groups["WasPrice"].Value)));
                pip.PicDetailPair.Add(new KeyValuePair<string, string>("Now price", TrimEndStart(match.Groups["NowPrice"].Value)));

                pi.PicInfoList.Add(pip);
            }

            return true;
        }

        private string TrimEndStart(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.TrimStart().TrimEnd();
            }

            return str;
        }
    }
}
