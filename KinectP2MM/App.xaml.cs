using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace KinectP2MM
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static String IMAGE_PATH;
        public static String FONT_FAMILY;
        public static String FONT_FAMILY_BOTTOM;
        public static String FONT_FAMILY_TOP;

        protected override void OnStartup(StartupEventArgs e)
        {
            IMAGE_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\KinectP2MM\\images";
            FONT_FAMILY = KinectP2MM.Properties.Settings.Default.font1_full;
            FONT_FAMILY_BOTTOM = KinectP2MM.Properties.Settings.Default.font1_bottom;
            FONT_FAMILY_TOP = KinectP2MM.Properties.Settings.Default.font1_top;

            if (IsKinectRuntimeInstalled)
            {
                //go ahead and load the StartupUri as defined in App.xaml
                base.OnStartup(e);
            }
            else
            {
                MessageBox.Show(Assembly.GetExecutingAssembly().FullName + " ne pourra pas fonctionner correctement."
                + " Une dépendance importante aurait dû être installé durant son installation: Microsoft Kinect Runtime 1.0");
            }
        }

        public bool IsKinectRuntimeInstalled
        {
            get
            {
                bool isInstalled;
                try
                {
                    TestForKinectTypeLoadException();
                    isInstalled = true;
                }
                catch (FileNotFoundException)
                {
                    isInstalled = false;
                }
                return isInstalled;
            }
        }

        // This Microsoft.Kinect.dll based type, must be isolated in its own method
        // as the CLR will attempt to load the Microsoft.Kinect.dll assembly it when this method is executed.
        private void TestForKinectTypeLoadException()
        {
            #pragma warning disable 219 //ignore the fact that status is unused code after this set.
            var status = KinectStatus.Disconnected;
            #pragma warning restore 219
        }
    }
}
