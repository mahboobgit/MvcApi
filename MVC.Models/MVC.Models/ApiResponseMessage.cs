using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MVC.Models
{
    public abstract class ApiResponseMessage<T>
    {
        public bool IsError { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public List<T> Content { get; set; }

        public abstract void InitializeContent();
    }

    
}
