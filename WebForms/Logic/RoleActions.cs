using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebForms.Models;

namespace WebForms.Logic
{
    internal class RoleActions
    {
        internal void createAdmin()
        {
            Models.ApplicationDbContext context = new Models.ApplicationDbContext();
            IdentityResult IdRoleResult;
            IdentityResult IdUserResult;

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleMgr = new RoleManager<IdentityRole>(roleStore);
            if (!roleMgr.RoleExists("Administrator"))
            {
                IdRoleResult = roleMgr.Create(new IdentityRole("Administrator"));
                if (!IdRoleResult.Succeeded)
                {

                }
            }
            var userMgr = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var appUser = new ApplicationUser()
            {
                UserName = "Admin",
            };
            IdUserResult = userMgr.Create(appUser, "Pa$$word");
            if (IdUserResult.Succeeded)
            {
                IdUserResult = userMgr.AddToRole(appUser.Id, "Administrator");
                if (!IdUserResult.Succeeded)
                {

                }
            }
            else
            {

            }
        }
    }
}