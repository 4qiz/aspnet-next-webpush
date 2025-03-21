using System.ComponentModel.DataAnnotations;

namespace WebPushApi.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Endpoint { get; set; }
        public string P256DH { get; set; }
        public string Auth { get; set; }
    }
}
