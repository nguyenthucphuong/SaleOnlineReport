using SaleOnline.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Utilities;
public partial class Customer
{
    [Key]
    public Guid CustomerId { get; set; }

    [StringLength(50)]
    public string FullName { get; set; } = "";

    [StringLength(50)]
    public string Address { get; set; } = "";

    [StringLength(10)]
    public string PhoneNumber { get; set; } = "";
    public Guid UserId { get; set; }
    public string Filter { get; set; } = "";

    public bool IsActive { get; set; } = true;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public Customer() {}

    public Customer(string fullName, string addRess, string phoneNumber, Guid userId, bool? isActive)
    {
        FullName = fullName.Trim();
        Address = addRess.Trim();
        PhoneNumber = phoneNumber.Trim();
        UserId = userId;
        Filter = FullName.ToLower() + " " + Address.ToLower() + " " + Utility.ConvertToUnsign(FullName.ToLower()) + " " + Utility.ConvertToUnsign(Address?.ToLower() ?? "") + " " + PhoneNumber;
        IsActive = isActive ?? true;
    }
}


//[StringLength(50)]
//public string FullName { get; set; } = "";

//[StringLength(50)]
//public string Address { get; set; } = "";

//[StringLength(10)]
//public string PhoneNumber { get; set; } = "";
//public string Filter { get; set; } = "";

//public bool IsActive { get; set; } = true;

//[ForeignKey("User")]
//public Guid UserId { get; set; }
//public virtual User User { get; set; } = null!;
//public Customer() {}

//public Customer(string fullName, string addRess, string phoneNumber, Guid userId, bool? isActive)
//{
//    FullName = fullName.Trim();
//    Address = addRess.Trim();
//    PhoneNumber = phoneNumber.Trim();
//    UserId = userId;
//    Filter = Utility.ConvertToUnsign(FullName.ToLower()) + " " + Utility.ConvertToUnsign(Address?.ToLower() ?? "") + " " + PhoneNumber;
//    IsActive = isActive ?? true;
//}



//[Key]
//[Column("CustomerId")]
//public int Id { get; set; }
//public Guid CustomerId { get; set; }