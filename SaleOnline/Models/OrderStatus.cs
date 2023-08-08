using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Common.Utilities;
namespace SaleOnline.Models;

[Table("OrderStatus")]
public partial class OrderStatus
{
    [Key]
    public Guid OrderStatusId { get; set; }

    [StringLength(50)]
    public string OrderStatusName { get; set; } = null!;

    public bool IsActive { get; set; }

    public string Filter { get; set; } = null!;

    [InverseProperty("OrderStatus")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public OrderStatus() { }
    public OrderStatus(string orderStatusName, bool isActive)
    {
        OrderStatusName = orderStatusName.Trim();
        Filter = OrderStatusName.ToLower() + " " + Utility.ConvertToUnsign(OrderStatusName.ToLower());
        IsActive = isActive;
    }
}
