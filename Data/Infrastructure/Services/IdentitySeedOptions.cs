﻿namespace Data.Infrastructure.Services;
public class IdentitySeedOptions
{
    public string AdminName { get; set; } = default!;
    public string OwnerName { get; set; } = default!;
    public string OwnerPassword { get; set; } = default!;
}