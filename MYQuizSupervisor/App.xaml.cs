using MYQuizSupervisor.Helpers;
using Xamarin.Forms;


namespace MYQuizSupervisor
{
    public partial class App : Application
    {
        public SupervisorTabbedView RootView {get; set;}
        public FinalSendView FinalSendView {get; set;}
        public LoginView LoginView { get; set; }

        public App()
        {
            InitializeComponent();

            RootView = new SupervisorTabbedView();
            FinalSendView = new FinalSendView();
            LoginView = new LoginView();
        }

        public void navigateTo(Page page)
        {
            MainPage = new NavigationPage(page);
        }

        protected override void OnStart()
        {
            if (Settings.ClientId == string.Empty)
            {
                this.navigateTo(this.LoginView);
            }
            else
            {
                this.navigateTo(this.RootView);
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
