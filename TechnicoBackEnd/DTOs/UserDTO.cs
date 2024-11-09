﻿using TechnicoBackEnd.Models;

namespace TechnicoBackEnd.DTOs;

public class UserDTO{
    public int Id { get; set; }

    public string? VAT { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    //public UserType? PType { get; set; } //TODO THINK ABOUT IT
}