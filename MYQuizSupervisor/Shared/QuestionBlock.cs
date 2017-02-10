using System.Collections.ObjectModel;

namespace MYQuizSupervisor
{
    public class QuestionBlock
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public ObservableCollection<Question> Questions { get; set; }
    }
}