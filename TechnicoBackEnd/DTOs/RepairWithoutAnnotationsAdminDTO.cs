using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicoBackEnd.DTOs;
public class RepairWithoutAnnotationsAdminDTO : RepairWithoutAnnotationsDTO
{
    public string? OwnerVAT { get; set; }

    public string? OwnerName { get; set; }

    public string? OwnerSurname { get; set; }

    public string? FullName => $"{OwnerName} {OwnerSurname}";
}
