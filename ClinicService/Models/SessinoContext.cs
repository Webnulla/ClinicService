namespace ClinicService.Models
{
    public class SessinoContext
    {
        public int SessionId { get; set; }

        public string SessionToken { get; set; }

        public AccountDto Account { get; set; }
    }
}
