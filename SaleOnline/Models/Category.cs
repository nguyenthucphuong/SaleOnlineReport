using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Common.Utilities;
namespace SaleOnline.Models;
using Newtonsoft.Json;
public partial class Category
{
    [Key]
    [Column("CategoryID")]
    public Guid CategoryId { get; set; }

    [StringLength(50)]
    public string CategoryName { get; set; } = null!;

    public string Filter { get; set; } = null!;

    public bool IsActive { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    
    
    public Category() {}
    public Category(string categoryName, bool isActive)
    {
        CategoryName = categoryName.Trim();
        Filter = CategoryName.ToLower() + " " + Utility.ConvertToUnsign(CategoryName.ToLower());
        IsActive = isActive;
    }

}



