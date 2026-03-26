namespace VotingSystem.Models
{
    public class Poll
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public List<PollOption> Options { get; set; }
        public List<Vote> Votes { get; set; } = new();
    }
}