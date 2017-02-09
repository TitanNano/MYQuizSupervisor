using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace MYQuizSupervisor
{

    public partial class SupervisorTabbedView : TabbedPage
    {

        private ObservableCollection<Group> _CurrentGroupList;

        private App App { get { return (MYQuizSupervisor.App)Application.Current; } }

        public NewQuestion NewQuestion { get; set; }

        public ObservableCollection<string> CurrentGroupSuggestions { get; set; }

        public ObservableCollection<Group> CurrentGroupList
        {
            get
            {
                return this._CurrentGroupList;
            }

            set
            {
                this._CurrentGroupList = value;
                OnPropertyChanged("CurrentGroupList");
            }
        }

        public SupervisorTabbedView()
        {
            InitializeComponent();

            this.NewQuestion = new NewQuestion { AnswerOptions = new ObservableCollection<LocalAnswerOption>() };
            this.CurrentGroupSuggestions = new ObservableCollection<string>();
            this.CurrentGroupList = new ObservableCollection<Group>();

            this.updateGroupList();
            this.BindingContext = this;

            this.ResetAnswerOptions();

            repeater.ItemsSource = this.NewQuestion.AnswerOptions;


            //Questions vom Server holen            
            updateQuestionBlock();          

        }
              
   
        void OnQuestionReadyToSend(object sender, System.EventArgs e)
        {
            App.MainPage.Navigation.PushAsync(App.FinalSendView);
        }

        void OnNewQuestionReadyToSend(object sender, System.EventArgs e)
        {
            var questionaier = new GivenAnswer
            {
                Group = this.CurrentGroupList.ToList().Find(item => item.Title == this.NewQuestion.Group),

                QuestionBlock = new QuestionBlock
                {
                    Questions = new ObservableCollection<Question>()
                }
            };

            var question = new Question
            {
                Text = this.NewQuestion.Text,

                AnswerOptions = new ObservableCollection<AnswerOption>(
                    this.NewQuestion.AnswerOptions
                    .Select<LocalAnswerOption, AnswerOption>(option => new AnswerOption { Text = option.Text })
                )
            };

            questionaier.QuestionBlock.Questions.Add(question);

            var duration = (Int64.Parse(this.NewQuestion.DurationMinutes) * 60) + Int64.Parse(this.NewQuestion.DurationSeconds);

            App.FinalSendView.prepareQuestionSending(questionaier, duration, false);
        }

        public void OnCurrentPageChanged(object sender, System.EventArgs e)
        {
            this.updateGroupList();

            this.updateQuestionBlock();            
        }

        

        private async void updateGroupList()
        {
            try
            {
                this.CurrentGroupList = await Networking.Current.getGroups();
            }
            catch
            {
                await this.DisplayAlert("Netzwerk Fehler", "Fehler beim Abrufen der Veranstalltungsliste!", "Ok");
            }
        }

        private async void updateQuestionBlock()
        {
            try
            {
                lv_fragen.ItemsSource = await Networking.Current.getPreparedQuestionBlocks();
            }
            catch
            {
                await this.DisplayAlert("Netzwerk Fehler", "Fehler beim Abrufen der Fragenliste!", "Ok");
            }
        }


        void OnGroupTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var groupAutoComplete = sender as AutoCompleteView;

            this.CurrentGroupSuggestions.Clear();

            foreach (var group in this.CurrentGroupList)
            {
                if (group.Title.Contains(groupAutoComplete.Text))
                {
                    this.CurrentGroupSuggestions.Add(group.Title);
                }
            } 
        }

        void OnAnswerTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {

            var answerOptions = this.NewQuestion.AnswerOptions;
            var hadEmpty = true;
            var lastItem = "";

            while (hadEmpty && answerOptions.Count > 1)
            {
                lastItem = answerOptions.Last().Text;
                var secondLastIndex = answerOptions.Count - 2;
                var secondLast = "";

                if (secondLastIndex >= 0)
                {
                    secondLast = answerOptions[secondLastIndex].Text;
                }

                if (lastItem == "" && secondLast == "")
                {
                    answerOptions.Remove(answerOptions.Last());
                    hadEmpty = true;
                }
                else 
                {
                    hadEmpty = false;
                }

            }

            lastItem = answerOptions.Last().Text;

            if (lastItem != "")
            {
                answerOptions.Add(new LocalAnswerOption());
            }

            repeater.ForceLayout();
        }

        public void ResetAnswerOptions() 
        {
            this.NewQuestion.AnswerOptions.Clear();
            this.NewQuestion.AnswerOptions.Add(new LocalAnswerOption());
        }
    }

    public class NewQuestion
    {
        public string Text { get; set; } = "";
        public ObservableCollection<LocalAnswerOption> AnswerOptions { get; set; }
        public string DurationMinutes { get; set; } = "";
        public string DurationSeconds { get; set; } = "";
        public string Group { get; set; } = "";
    }

    public class LocalAnswerOption
    {
        public string Text { get; set; } = "";
    }
}
