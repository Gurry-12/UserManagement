using System;

namespace UserManagement.WebAPI.Controllers
{
    /// <summary>
    /// Custom attribute for documenting API response status codes in Swagger/OpenAPI
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerResponseAttribute : Attribute
    {
        /// <summary>
        /// Gets the HTTP status code
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the description of the response
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerResponseAttribute"/> class
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <param name="description">The description of the response</param>
        public SwaggerResponseAttribute(int statusCode, string description)
        {
            StatusCode = statusCode;
            Description = description;
        }
    }
}