using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SaleOnline.Models;

public partial class Cart
{
    [Key]
    public Guid ProductId { get; set; }

    public string? ProductName { get; set; } = "";

    public string? ProductImage { get; set; } = "";

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ProductPrice { get; set; }

    public int Quantity { get; set; }

    public string? SessionId { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
    public Guid UserId { get; set; }
    public Cart()
    {
        ProductName = "";
    }
    public decimal Total => ProductPrice * Quantity;
    public decimal TotalDiscount => ((Product?.Promotion?.Discount ?? 0)* ProductPrice * Quantity)/100;
}

