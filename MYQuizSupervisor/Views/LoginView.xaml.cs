using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MYQuizSupervisor
{
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();

            btn_login_weiter.HeightRequest = btn_login_weiter.Height;
        }
    }
}
