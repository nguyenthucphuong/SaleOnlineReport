using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Common.Utilities;
namespace SaleOnline.Models;

public partial class Role
{
    [Key]
    public Guid RoleId { get; set; }

    [StringLength(50)]
    public string RoleName { get; set; } = null!;

    public string Filter { get; set; } = null!;

    public bool IsActive { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public Role() { }
    public Role(string roleName, bool isActive)
    {
        RoleName = roleName.Trim();
        Filter = Utility.ConvertToUnsign(RoleName.ToLower());
        IsActive = isActive;
    }
}
