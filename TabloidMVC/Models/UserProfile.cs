using System.ComponentModel;

namespace TabloidMVC.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DisplayName("Username")]
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string ImageLocation { get; set; }
        [DisplayName("User Type")]
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }
        [DisplayName("Name")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
