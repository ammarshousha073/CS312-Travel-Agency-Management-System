using TravelAgencyManagementSystem.Forms;

namespace TravelAgencyManagementSystem;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new LoginForm());
    }    
}
