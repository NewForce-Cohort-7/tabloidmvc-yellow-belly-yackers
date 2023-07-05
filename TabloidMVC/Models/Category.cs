using System.ComponentModel;

namespace TabloidMVC.Models
{
    public class Category
    {
        public int Id { get; set; }

        [DisplayName("Category")]
        public string Name { get; set; }

    }
}
