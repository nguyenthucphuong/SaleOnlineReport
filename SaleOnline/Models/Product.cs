using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Common.Utilities;
namespace SaleOnline.Models;

public partial class Product
{
    [Key]
    [Column("ProductID")]
    public Guid ProductId { get; set; }

    public Guid? UserId { get; set; }

    [Column("CategoryID")]
    public Guid? CategoryId { get; set; }

    [Column("PromotionID")]
    public Guid? PromotionId { get; set; }

    [StringLength(100)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? ProductPrice { get; set; }

    public string? ProductDes { get; set; } = "";

    public string? ProductImage { get; set; }

    public bool IsNew { get; set; }

    public bool IsSale { get; set; }

    public bool IsPro { get; set; }

    public string Filter { get; set; } = null!;

    public bool IsActive { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("PromotionId")]
    [InverseProperty("Products")]
    public virtual Promotion? Promotion { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    [ForeignKey("UserId")]
    [InverseProperty("Products")]
    public virtual User? User { get; set; }
    public Product() {}
    
    public Product(Guid? userId, Guid? categoryId, Guid? promotionId, string productName, decimal? productPrice, string? productDes, string? productImage, bool? isNew, bool? isSale, bool? isPro, string? filter, bool? isActive)
    {
        UserId = userId;
        CategoryId = categoryId;
        PromotionId = promotionId;
        ProductName = productName.Trim();
        ProductPrice = productPrice;
        ProductDes = productDes?.Trim();
        ProductImage = productImage;
        IsNew = isNew ?? false;
        IsSale = isSale ?? false;
        IsPro = isPro ?? false;
        Filter = ProductName.ToLower() + " " + ProductDes?.ToLower() + " " + Utility.ConvertToUnsign(ProductName.ToLower()) + " " + Utility.ConvertToUnsign(productDes?.ToLower() ?? "");
        IsActive = isActive ?? true;
    }

}


