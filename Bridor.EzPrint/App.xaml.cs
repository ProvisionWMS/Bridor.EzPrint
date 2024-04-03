using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Bridor.EzPrint.Helpers;

namespace Bridor.EzPrint
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Mutex variable to keep a single instance of the application
        /// </summary>
        private static Mutex instanceMutex;
        /// <summary>
        /// Initialzied the applicaiton object
        /// </summary>
        public App() {
            // variable to know if this is the first instance
            bool firstInstance;

            instanceMutex = new Mutex(true, String.Format(@"Global\{0}", Assembly.GetExecutingAssembly().GetName().Name), out firstInstance);
            if (!firstInstance) {
                // This is a second instance, do not allow it to run
                Application.Current.Shutdown();
                return;
            }
        }
        /// <summary>
        /// Handles the application start up
        /// </summary>
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            LogWriter.Instance.MessageFormat("App.OnStartUp", "Starting Bridor EzPrint v{0}",
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
        }
        /// <summary>
        /// Handles the applicaiton exit
        /// </summary>
        protected override void OnExit(ExitEventArgs e) {
            LogWriter.Instance.MessageFormat("App.OnExit", "Closing Bridor EzPrint v{0}",
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            base.OnExit(e);
        }
    }
}
