﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechnicoBackEnd.DTOs;
public class RepairDeactivateRequestDTO
{
    public int RepairId { get; set; }
    public string? OwnerVAT { get; set; }
}
