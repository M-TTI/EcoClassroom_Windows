using EcoClassroom_Windows.Data;
using EcoClassroom_Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace EcoClassroom_Windows
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            InitializeDatabase();

            Application.Run(new LoginForm());
        }

        private static void InitializeDatabase()
        {
            try
            {
                using var context = new AppDbContext();
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'initialisation de la base de données : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}