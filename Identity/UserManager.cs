using GiveLoveFirst.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveLoveFirst.Identity
{
    public class ApplicationUserManager : UserManager<User, int>
    {
        public ApplicationUserManager(IUserStore<User, long> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new IdentityUserStore);

            // Configure the application user manager
            manager.UserValidator = new UserValidator<User, long>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            manager.PasswordValidator = new PasswordValidator
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireLowercase = false,
                RequireNonLetterOrDigit = true,
                RequireUppercase = false
            };

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User, long>(dataProtectionProvider.Create("TEST"));
            }

            return manager;
        }
    }
}