using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.ExceptionHandling
{
    public class ValidationException1
    {
        ValidationException1(string message)
        {
            throw new Exception(message);
        }
    }
}
