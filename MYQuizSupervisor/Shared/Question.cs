using System.Collections.ObjectModel;

namespace MYQuizSupervisor
{
    public class Question
    {
        public long Id { get; set; }
        public string Category { get; set; }
        public string MultipleChoice { get; set; }
        public string Text { get; set; }
        public ObservableCollection<AnswerOption> AnswerOptionList { get; set; }
    }
}