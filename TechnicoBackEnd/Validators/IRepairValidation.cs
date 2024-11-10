using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators;
public interface IRepairValidation
{
    ResponseApi<RepairDTO>? RepairValidatorUser(RepairDTO repair);
    ResponseApi<AdminCreateUpdateRepairDTO>? RepairValidatorAdmin(AdminCreateUpdateRepairDTO repair);
}