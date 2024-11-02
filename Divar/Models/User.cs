namespace Divar.Models
{
    public class User : IdentityUser
    {
        [Required]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "وارد کردن نام الزامی است")]
        public string FirstName { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "وارد کردن نام خانوادگی الزامی است")]
        public string LastName { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "وارد کردن ایمیل الزامی است")]
        public string Email { get; set; }
        [MaxLength(12)]
        [Required(ErrorMessage = "وارد کردن شماره تلفن الزامی است")]
        public string PhoneNumber { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "وارد کردن رمز عبور الزامی است")]
        public string PasswordHash { get; set; }

        public ICollection<Advertisement> ?Advertisements { get; set; }
    }
}
