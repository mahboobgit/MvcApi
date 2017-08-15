namespace MVC.Models
{

    using System.Collections.Generic;

    public class Error
    {
        public string Message { get; set; }
    }

    public class ErrorResponse : ApiResponseMessage<Error>
    {
        public ErrorResponse()
        {
            this.InitializeContent();
        }

        public override void InitializeContent()
        {
            this.Content = new List<Error>();
        }
    }
}