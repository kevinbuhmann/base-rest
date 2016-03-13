using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GiveLoveFirst.Identity.Models
{
    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>, IUser<int>
    {
    }
}