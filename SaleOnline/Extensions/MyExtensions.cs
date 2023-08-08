using SaleOnline.Models;

namespace SaleOnline.Extensions
{
    public static class MyExtensions
    {
        public static string GetIsActiveClass(this Category category)
        {
            return GetIsActiveClass(category.IsActive);
        }

        public static string GetIsActiveText(this Category category)
        {
            return GetIsActiveText(category.IsActive);
        }

        public static string GetIsActiveClass(this Product product)
        {
            return GetIsActiveClass(product.IsActive);
        }

        public static string GetIsActiveText(this Product product)
        {
            return GetIsActiveText(product.IsActive);
        }

        public static string GetIsActiveClass(this Promotion promotion)
        {
            return GetIsActiveClass(promotion.IsActive);
        }

        public static string GetIsActiveText(this Promotion promotion)
        {
            return GetIsActiveText(promotion.IsActive);
        }

        public static string GetIsActiveClass(this User user)
        {
            return GetIsActiveClass(user.IsActive);
        }

        public static string GetIsActiveText(this User user)
        {
            return GetIsActiveText(user.IsActive);
        }

        public static string GetIsActiveClass(this Role role)
        {
            return GetIsActiveClass(role.IsActive);
        }

        public static string GetIsActiveText(this Role role)
        {
            return GetIsActiveText(role.IsActive);
        }

        private static string GetIsActiveClass(bool? isActive)
        {
            switch (isActive)
            {
                case true:
                    return "pd-setting";
                case false:
                    return "ds-setting";
                default:
                    return "ds-setting";
            }
        }

        private static string GetIsActiveText(bool? isActive)
        {
            switch (isActive)
            {
                case true:
                    return "Kích hoạt";
                case false:
                    return "Tạm khóa";
                default:
                    return "Lỗi";
            }
        }
    }
}
