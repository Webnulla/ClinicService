namespace ClinicService.Models.Requests
{
    public class AuthenticationResponse
    {
        public AuthenticationStatus Status { get; set; }

        public SessinoContext SessionContext { get; set; }
    }
}
