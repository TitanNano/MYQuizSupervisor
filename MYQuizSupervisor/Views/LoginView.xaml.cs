using System;
using System.Collections.Generic;
using System.ComponentModel;
using MYQuizSupervisor.Helpers;
using Xamarin.Forms;

namespace MYQuizSupervisor
{
    public partial class LoginView : ContentPage
    {
        private App App { get { return (MYQuizSupervisor.App)App.Current; } }

        public string currentPassword { get; set; }

        private Boolean _loginNotPending = true;

        public Boolean loginNotPending
        {
            get
            {
                return this._loginNotPending;
            } 

            set {
                this._loginNotPending = value;
                this.OnPropertyChanged("loginNotPending");
            } 
        }

        public LoginView()
        {
            InitializeComponent();

            this.BindingContext = this;
            btn_login_weiter.HeightRequest = btn_login_weiter.Height;
        }

        async void PasswordReadyToLogin(object sender, System.EventArgs e)
        {
            this.loginNotPending = false;

            try
            {
                var registration = await Networking.Current.registerClientDevice(null, this.currentPassword);

                Settings.ClientId = registration.id;

                App.navigateTo(App.RootView);
            }
            catch
            {
                this.loginNotPending = true;

                await this.DisplayAlert("Anmeldung Fehlgeschlagen", "Das eingegebene Passwort ist falsch!", "Doof!");
            }
        }
    }
}
