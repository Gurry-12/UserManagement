using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Application.Exceptions
{
    public class CustomValidationException : Exception
    {
        public List<string> Errors { get; }

        public CustomValidationException(List<string> errors)
            : base("Validation failed for one or more fields.")
        {
            Errors = errors;
        }
    }
}
