using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicoBackEnd.DTOs;
public class RepairAdminCreateUpdateDTO : RepairDTO
{
    public string? OwnerVAT { get; set; }
}
