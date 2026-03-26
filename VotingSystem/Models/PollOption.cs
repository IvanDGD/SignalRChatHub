namespace VotingSystem.Models
{
    public class PollOption
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string Text { get; set; }

        public Poll Poll { get; set; }
        public List<Vote> Votes { get; set; }
    }
}