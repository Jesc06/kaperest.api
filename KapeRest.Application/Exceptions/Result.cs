using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Exceptions
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public T Value { get; set; }

        public static Result<T> Ok(T value)
        {
            return new Result<T>
            {
                Success = true,
                Value = value
            };
        }

        public static Result<T> Fail(string error)
        {
            return new Result<T>
            {
                Success = false,
                Error = error
            };
        }

    }
}
