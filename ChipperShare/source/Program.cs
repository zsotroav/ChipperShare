using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChipperShare
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Contains("-server"))
            {
                for (int i = 0; i + 1 < args.Length; i++)
                {
                    if (args[i] == "-server")
                        Application.Run(new FormServer(true, args[i+1])); ;
                }

            }
            else
                Application.Run(new Form1());
        }
    }
}
