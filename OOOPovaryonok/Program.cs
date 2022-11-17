using OOOPovaryonok.Classes;
using System;
using System.Windows.Forms;

namespace OOOPovaryonok
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SQLDatabaseHelper.DatabaseContext = new Entities.Trade();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Authorization());
        }
    }
}
