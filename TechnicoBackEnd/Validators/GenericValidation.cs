using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators;

public class GenericValidation
{
    public static ResponseApi<bool> IsNull(object? Member)
    {
        int status = (Member == null) ? 1 : 0;
        string description = (Member == null) ? "Parameter not found" : "";
        bool value = (Member == null);

        return new ResponseApi<bool> { Description = description, Status = status, Value = value };
    }
}
