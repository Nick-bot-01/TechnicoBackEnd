using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators;
public interface IRepairValidation
{
    ResponseApi<RepairDTO>? RepairValidator(RepairDTO repair);
}