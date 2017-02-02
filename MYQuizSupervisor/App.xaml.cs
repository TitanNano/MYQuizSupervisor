using Xamarin.Forms;


namespace MYQuizSupervisor
{
    public partial class App : Application
    {
        public SupervisorTabbedView RootView {get; set;}
        public FinalSendView FinalSendView {get; set;}

        public App()
        {
            InitializeComponent();

            RootView = new SupervisorTabbedView();
            FinalSendView = new FinalSendView();

            MainPage = new NavigationPage(RootView);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
