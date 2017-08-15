using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MVC.Models;

namespace MVC.Infrastructure
{
    interface IWebApiClient
    {
        Task<HttpResponseMessage> PostJsonEncodedContent<T>(string requestUri, T content) where T : class;

        Task<HttpResponseMessage> GetJsonEncodedContent(string requestUri);
    }
}
