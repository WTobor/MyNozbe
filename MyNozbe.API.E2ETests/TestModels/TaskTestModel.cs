namespace MyNozbe.API.E2ETests.TestModels
{
    public class TaskTestModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsCompleted { get; set; }
        
        public int? ProjectId { get; set; }
    }
}