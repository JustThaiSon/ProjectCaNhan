using System.ComponentModel.DataAnnotations; // tag key
using System.ComponentModel.DataAnnotations.Schema; // tag table
namespace project_ca_nhan.Models
{
    [Table("Categories")]
    public class ItemCategory
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public int DisplayHomePage { get; set; }
    }
}
