using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace MYQuizSupervisor
{
    public partial class FinalSendView : ContentPage
    {
        private long Duration = 0;
        private long _EndTime = 0;
        private bool _isBusy = false;
        private long _arrivedAnswers = 0;

        public GivenAnswer Questionair { get; set; }
        public Boolean AllowSingleTopic { get; set; }
        public Websockets.IWebSocketConnection CurrentSocket { get; set; }

        public long QuestionEndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                _EndTime = value; 
                OnPropertyChanged("QuestionEndTime");
            }
        }

        public Boolean isBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;

                OnPropertyChanged("isBusy");
                OnPropertyChanged("isNotBusy");
            }
        }

        public Boolean isNotBusy
        {
            get
            {
                return !_isBusy;
            }
        }

        public string QuestionTitle
        {
            get
            {
                if (this.Questionair != null)
                {
                    if (this.Questionair.QuestionBlock.Title != null && this.Questionair.QuestionBlock.Title != "")
                    {
                        return this.Questionair.QuestionBlock.Title;
                    }

                    return this.Questionair.QuestionBlock.Questions.First().Text;
                }

                return "";
            }
        }

        public string GroupName
        {
            get
            {
                if (this.Questionair != null)
                {
                    return this.Questionair.Group.Title + " (" + this.Questionair.Group.EnterGroupPin + ")";
                }

                return "";
            }
        }

        public string RemainingTime
        {
            get
            {
                long duration = 0;

                if (this.QuestionEndTime > 0)
                {
                    duration = this.QuestionEndTime - this.getUnixTimestamp();
                }
                else
                {
                    duration = this.Duration;
                }

                var minutes = Math.Floor((decimal)duration / 60);
                var seconds = duration - (minutes * 60);

                var result = String.Format("{0:00}:{1:00}", minutes, seconds);

                return result;
            }
        }

        private App App
        {
            get
            {
                return (MYQuizSupervisor.App)App.Current;
            }
        }

        public long arrivedAnswers
        {
            get
            {
                return this._arrivedAnswers;
            }
            set
            {
                this._arrivedAnswers = value; OnPropertyChanged("arrivedAnswers");
            }
        }

        public FinalSendView()
        {
            InitializeComponent();

            this.BindingContext = this;

            // @Todo: disable back button once the questionaier has been sent out.
            //NavigationPage.SetHasBackButton(this, false);
        }

        public long getUnixTimestamp()
        {
            long unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;

            return unixTimestamp;
        }

        public Boolean OnTimerTick()
        {
            var now = this.getUnixTimestamp();
            var status = now < this.QuestionEndTime;

            OnPropertyChanged("RemainingTime");

            if (!status)
            {
                NavigationPage.SetHasBackButton(this, true);
                this.CurrentSocket.Close();
                this.isBusy = false;
            }

            return status;
        }

        protected override bool OnBackButtonPressed()
        {
            if (this.isBusy)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await this.DisplayAlert("Frage Abbrechen", "Wollen Sie wirklich die aktuelle Frage abbrechen?", "Ja", "Nein");

                    if (result)
                    {
                        await App.MainPage.Navigation.PopAsync();
                    }
                });

                return true;
            }

            return false;
        }

        public void prepareQuestionSending(GivenAnswer questionaier, long duration, Boolean allowSingleTopic = true)
        {
            this.Questionair = questionaier;
            this.Duration = duration;
            this.AllowSingleTopic = allowSingleTopic;

            OnPropertyChanged("RemainingTime");
            OnPropertyChanged("Questionair");
            OnPropertyChanged("GroupName");
            OnPropertyChanged("QuestionTitle");

            App.MainPage.Navigation.PushAsync(this);
        }

        async void OnSendQuestion(object sender, System.EventArgs e)
        {
            this.QuestionEndTime = this.getUnixTimestamp() + this.Duration;

            Xamarin.Forms.Device.StartTimer(new TimeSpan(0, 0, 1), this.OnTimerTick);
            NavigationPage.SetHasBackButton(this, false);

            this.Questionair.TimeStamp = this.QuestionEndTime.ToString();
            this.isBusy = true;

            var result = await Networking.Current.sendQuestion(this.Questionair);
            OnPropertyChanged("Questionair");
            this.Questionair = result;

            this.CurrentSocket = Websockets.WebSocketFactory.Create();
            this.CurrentSocket.Open("http://h2653223.stratoserver.net/ws/" + result.SurveyId);

            var currentCount = 0;

            this.arrivedAnswers = 0;

            this.CurrentSocket.OnMessage += (string obj) => {
                var answer = JsonConvert.DeserializeObject<GivenAnswer>(obj);

                if (answer != null)
                {
                    currentCount += 1;
                    this.arrivedAnswers = currentCount;
                }
            };
         }
    }
}
