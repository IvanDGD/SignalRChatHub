namespace VotingSystem.Models
{
    public class Vote
    {
        public int Id { get; set; }

        public int PollId { get; set; }
        public int PollOptionId { get; set; }
        public string UserId { get; set; }

        public Poll Poll { get; set; }
        public PollOption PollOption { get; set; }
        public User User { get; set; }
    }
}