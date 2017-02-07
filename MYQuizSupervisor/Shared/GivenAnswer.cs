using System;
using System.Collections.Generic;

namespace MYQuizSupervisor
{
    public partial class GivenAnswer
    {
        public long Id { get; set; }
        public long DeviceId { get; set; }
        public long GroupId { get; set; }
        public long SingleTopicId { get; set; }
        public long QuestionBlockId { get; set; }
        public long QuestionId { get; set; }
        public long AnswerOptionId { get; set; }
        public string TimeStamp { get; set; }
        public Device Device;
        public Group Group;
        public SingleTopic SingleTopic;
        public AnswerOption AnswerOption;
        public Question Question;
        public QuestionBlock QuestionBlock;
    }
}