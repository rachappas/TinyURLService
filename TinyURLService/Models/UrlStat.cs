
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TinyURLService.Models
{
    public class UrlStats
    {
        public const string UNKNOWN = "Unknown";
        public const string HTTP = "http://";
        public const string HTTPS = "https://";

        public int UrlStatId { get; set; }
        [DisplayName("User Agent")]
        public string UserAgent { get; set; }
        [DisplayName("Host Address")]
        public string UserHostAddress { get; set; }
        [DisplayName("Language")]
        public string UserLanguage { get; set; }
        [DisplayName("Refferal URL")]
        public string UrlRefferer { get; set; }
        [DisplayName("Mobile")]
        public bool IsMobile { get; set; }
        public string Browser { get; set; }
        [DisplayName("Browser Version")]
        public int MajorVersion { get; set; }
        public int UrlId { get; set; }
        public virtual URL Url { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public UrlStats()
        {

        }
        /// <summary>
        /// Bind properties with data that is coming from the request
        /// </summary>
        /// <param name="request">HTTP request</param>
        public UrlStats(HttpRequestBase request)
        {
            if (string.IsNullOrEmpty(request.UrlReferrer.Host))
                UrlRefferer = UNKNOWN;
            else
                UrlRefferer = request.UrlReferrer.Host;

            if (string.IsNullOrEmpty(request.UserAgent))
                UserAgent = UNKNOWN;
            else
                UserAgent = request.UserAgent;

            if (string.IsNullOrEmpty(request.UserHostAddress))
                UserHostAddress = UNKNOWN;
            else
                UserHostAddress = request.UserHostAddress;

            if (string.IsNullOrEmpty(request.UserLanguages[0]))
                UserLanguage = UNKNOWN;
            else
                UserLanguage = request.UserLanguages[0];
            
            if (string.IsNullOrEmpty(request.Browser.Browser))
                Browser = UNKNOWN;
            else
                Browser = request.Browser.Browser;
            
            MajorVersion = request.Browser.MajorVersion;
            IsMobile = request.Browser.IsMobileDevice;
        }
    }
}