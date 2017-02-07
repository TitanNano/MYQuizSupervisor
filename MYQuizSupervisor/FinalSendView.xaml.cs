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
        private long Duration;

        public GivenAnswer Questionair { get; set; }
        public Boolean AllowSingleTopic { get; set; }

        public string RemainingTime
        {
            get
            {
                if (this.Duration > 0)
                {
                    var minutes = Math.Floor((decimal)this.Duration / 60);
                    var seconds = this.Duration - (minutes * 60);

                    var result = String.Format("{0:00}:{1:00}", minutes, seconds);

                    return result;
                }

                return "00:00";
            }
        }

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

            this.BindingContext = this;

            // @Todo: disable back button once the questionaier has been sent out.
            //NavigationPage.SetHasBackButton(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => {
                var result = await this.DisplayAlert("Frage Abbrechen", "Wollen Sie wirklich die aktuelle Frage abbrechen?", "Ja", "Nein");

                if (result)
                {
                    await App.MainPage.Navigation.PopAsync();
                }
            });

            return true;
        }

        public void prepareQuestionSending(GivenAnswer questionaier, long duration, Boolean allowSingleTopic = true)
        {
            this.Questionair = questionaier;
            this.Duration = duration;
            this.AllowSingleTopic = allowSingleTopic;

            OnPropertyChanged("RemainingTime");

            App.navigateTo(this);
        }

        void OnSendQuestion(object sender, System.EventArgs e)
        {
            App.MainPage.Navigation.PopAsync();
        }
    }
}
