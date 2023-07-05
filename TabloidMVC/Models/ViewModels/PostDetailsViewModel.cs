using System.ComponentModel;

namespace TabloidMVC.Models.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post? Post { get; set; }
        public List<Tag>? Tags { get; set; }

        [DisplayName("Author")]
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public int Id { get; set; }


    }
}
