using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicoBackEnd.DTOs;
public class RepairWithAdminDetailsDTO : RepairDTO
{
    public string? OwnerName { get; set; } //ayta ta epistrefw se view tou admin
    public string? Address { get; set; }
}
