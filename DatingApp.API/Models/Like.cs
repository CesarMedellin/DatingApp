namespace DatingApp.API.Models
{
    public class Like
    { // liker = yo likee = quien me gusta
        public int LikerId { get; set; }
        public int LikeeId { get; set; }

        public User Liker { get; set; }

        public User Likee { get; set; }
    }
}