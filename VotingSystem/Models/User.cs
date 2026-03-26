using Microsoft.AspNetCore.Identity;

namespace VotingSystem.Models
{
    public class User : IdentityUser
    {
        public List<Vote> Votes { get; set; }
    }
}