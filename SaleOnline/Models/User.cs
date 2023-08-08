using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace SaleOnline.Models;

public partial class User
{
    [Key]
    public Guid UserId { get; set; }

    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [MaxLength(100)]
    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid RoleId { get; set; }

    public string Filter { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("User")]
    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    [ForeignKey("RoleId")]

    public virtual Role Role { get; set; } = null!;
    [InverseProperty("User")]
    public virtual Customer Customer { get; set; } = null!;
    public User() {}

    public User(string userName, string password, string email, Guid roleId, bool? isActive)
    {
        UserName = userName;
        Password = password;
        Email = email;
        RoleId = roleId;
        Filter = UserName.ToLower() + " " + Email.ToLower();
        IsActive = isActive ?? true;
    }


}
