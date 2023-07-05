

namespace TabloidMVC.Models.ViewModels
{
    public class PostTagViewModel
    {
        public Post Post { get; set; }
        public PostTag PostTag { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
