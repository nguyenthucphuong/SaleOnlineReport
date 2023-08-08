using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Common.Utilities;
namespace SaleOnline.Models;

public partial class Payment
{
    [Key]
    public Guid PaymentId { get; set; }

    [StringLength(50)]
    public string PaymentName { get; set; } = null!;

    public bool IsActive { get; set; }

    public string Filter { get; set; } = null!;

    [InverseProperty("Payment")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public Payment() {}
    public Payment(string paymentName, bool isActive)
    {
        PaymentName = paymentName.Trim();
        Filter = PaymentName + " " + Utility.ConvertToUnsign(PaymentName.ToLower());
        IsActive = isActive;
    }
}
