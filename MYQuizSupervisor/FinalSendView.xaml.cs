using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MYQuizSupervisor
{
    public partial class FinalSendView : ContentPage
    {
        private App App
        {
            get
            {
                return (MYQuizSupervisor.App)App.Current;
            }
        }

        public FinalSendView()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await this.DisplayAlert("Frage Abbrechen", "Wollen Sie wirklich die aktuelle Frage abbrechen?", "Ja", "Nein");

                if (result)
                {
                    await App.MainPage.Navigation.PopAsync();
                }
            });

            return true;
        }

        void OnSendQuestion(object sender, System.EventArgs e)
        {
            App.MainPage.Navigation.PopAsync();
        }
    }
}
