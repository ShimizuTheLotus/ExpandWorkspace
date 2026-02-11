using System.Configuration;
using System.Data;
using System.Windows;


namespace ExpandWorkspace
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        System.Threading.Mutex? mutex;

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "ShimizuTheLotus.ExpandWorkspace", out ret);

            if (!ret)
            {
                System.Windows.MessageBox.Show("ExpandWorkspace已有一个程序实例运行");
                Environment.Exit(0);
            }

        }
    }

}
