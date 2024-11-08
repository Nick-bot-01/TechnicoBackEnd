namespace TechnicoBackEnd.DTOs;

public class UserWithPropertiesDTO : UserDTO{
    public List<PropertyDTO> Properties { get; set; } = [];
}
