namespace MYQuizSupervisor
{
    public class Group
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string EnterGroupPin { get; set; }
        public int? DeviceCount { get; set; }

        //zum Deserialisieren
        public Group() { }
    }
}