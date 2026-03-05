using System.ComponentModel.DataAnnotations;
using Ecommerce.Shared.Enums;

namespace Ecommerce.Shared.Entities
{
    public class Order
    {
        [Key]
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
