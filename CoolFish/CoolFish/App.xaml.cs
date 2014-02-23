using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace CoolFishNS
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    internal partial class App
    {
        static App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("GreyMagic"))
            {
                return Assembly.Load(CoolFishNS.Properties.Resources.GreyMagic);
            }
            if (args.Name.Contains("MahApps"))
            {
                return Assembly.Load(CoolFishNS.Properties.Resources.MahApps_Metro);
            }
            if (args.Name.Contains("Interactivity"))
            {
                return Assembly.Load(CoolFishNS.Properties.Resources.System_Windows_Interactivity);
            }
          
            return null;
        }
    }
}