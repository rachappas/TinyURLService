﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net;

namespace TinyURLService.Models
{
    public class URLS
    {
        public const string UNKNOWN = "Unknown";
        public const string HTTP = "http://";
        public const string HTTPS = "https://";

        [Key]
        public int UrlId { get; set; }
        [Required]
        [DisplayName("Long URL")]
        public string LongUrl { get; set; }
        [Required]
        [DisplayName("Short URL")]
        public string ShortUrl { get; set; }
        [Required]
        [DisplayName("Clicks")]
        public int Hits { get; set; }

        [Required]
        [DisplayName("Created")]
        public DateTime GeneratedDate { get; set; }
        public string UserId { get; set; }

        public virtual List<UrlStat> UrlStats { get; set; }

        /// <summary>
        /// Generate random URL ID which is used to create unique URL
        /// </summary>
        public void GenerateRandomShortUrl()
        {
            string number = "";
            int j;
            Random random = new Random();

            for (int i = 0; i < 6; i++)
            {
                j = random.Next(0, 35);
                if (j < 10)
                    j += 48;
                else
                    j += 87;
                number = number + char.ConvertFromUtf32(j);
            }
            ShortUrl = number;
        }

        /// <summary>
        /// Check if provided URL is valid HTTP url
        /// </summary>
        /// <param name="url">URL (Uniform resource locator)</param>
        /// <returns>true or false depends on the URL contains HTTP</returns>
        public bool HasHTTPProtocol(string url)
        {
            url = url.ToLower();
            if (url.Length > 5)
            {
                if (url.StartsWith(HTTP) || url.StartsWith(HTTPS))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Check whether provided URL exists by doing request to it and waiting for response.
        /// </summary>
        /// <returns>true or false depends on the availability of the provided URL</returns>
        public bool CheckLongUrlExists()
        {
            int linkLength = LongUrl.Length;
            if(!HasHTTPProtocol(LongUrl))
                LongUrl = HTTP + LongUrl;

            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(LongUrl) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }
    }
}