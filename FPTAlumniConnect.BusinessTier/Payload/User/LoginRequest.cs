using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTAlumniConnect.BusinessTier.Payload.User
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is missing")]
        [MaxLength(50, ErrorMessage = "Email's max length is 50 characters")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is missing")]
        [MaxLength(64, ErrorMessage = "Password's max length is 64 characters")]
        public string Password { get; set; }
    }
}
