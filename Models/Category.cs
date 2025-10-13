using System.ComponentModel.DataAnnotations;
namespace Bulky.Models
{
    public class Category
    {
       
        public int Id { get; set; }

        [Required,Display(Name="Category Name"),MinLength(2,ErrorMessage ="length should be at least 2"),MaxLength(10,ErrorMessage ="Name should be less than 10 characters")]
        public string Name { get; set; }
        [Required,Display(Name ="Display Orderid"),Range(1,100,ErrorMessage ="Display order must be between 1 to 100")]
        public int DisplayOrder { get; set; }
    }
}
