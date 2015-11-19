using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TinyURLService.Models;

namespace TinyURLService.DAL
{
    public class URLContext
    {
        public class UrlContext :DbContext 
        {
            public DbSet<URL> Urls { get; set; }
            public DbSet<UrlStat> UrlStats { get; set; }
        }
    }
}