using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace project_ca_nhan.Models
{
    [Table("Respond")]
    public class ItemRespond
    {
        [Key]
        public int Id { get; set; }
        
        public string Message {  get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
    }
}
