using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Helpers;

namespace TechnicoBackEnd.Services;

public class RepairService
{
    private readonly TechnicoDbContext db;

    public RepairService(TechnicoDbContext db)
    {
        this.db = db;
    }


    public List<RepairDTO> GetAllPendingRepairs(RepairStatus Status)
    {
        return db.Repairs.Where(x => x.RepairStatus == Status)
            .Select(x => x.ConvertRepair()).ToList();
    }

    public List<RepairDTO> GetAllOwnerRepairsByVAT(int VATNum)
    {
        return db.Repairs.Where(x => x.VATNum == VATNum)
            .Select(x => x.ConvertRepair()).ToList();
    }

    public List<RepairDTO> GetAllOwnerRepairsByDateOrRangeOfDates(DateTime StartDate,DateTime EndDate)
    {
        if (EndDate == null)
        {
            return db.Repairs.Where(x => x.Date = StartDate)
                .Select(x => x.ConvertRepair()).ToList();
        }

        return db.Repairs.Where(x=>x.Date>= StartDate && x.Date<=EndDate)
            .Select(x=> x.ConvertRepair()).ToList();
    }

    public Repair? GetRepairByID(int Id)
    {
        return db.Repairs.Where(x=>x.Id == Id)
            .Select(x=>x.ConvertProperty())
            .FirstOrDefault();
    }
}
