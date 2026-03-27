using System;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;

namespace Basics
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ient = new GenericIdentity("Pietje");
            var pri = new GenericPrincipal(ient, new string[] { "Admins"});

            Thread.CurrentPrincipal = pri;


            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            IPrincipal prin = Thread.CurrentPrincipal;
            IIdentity identity = prin.Identity;

          //  foreach(var role in prin.)

            bool isZo =  prin.IsInRole("Administrator");
            Console.WriteLine(isZo);
            Console.WriteLine(identity.Name);

            DoeIets();
        }

        //[CodeAccessSecurity(SecurityAction.Demand]6
        [PrincipalPermission(SecurityAction.Demand, Role ="Admin")]
        static void DoeIets()
        {
            Console.WriteLine("Doet Iets");
        }
    }
}
