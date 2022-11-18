using ClinicService.Data;
using ClinicService.Models.Requests;
using ClinicService.Models;
using ClinicService.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClinicService.Services.Impl
{
    public class AuthenticateService : IAuthenticateService
    {
        public const string SecretKey = "kYp3s6v9y/B?E(H+";

        #region Services

        private readonly IServiceScopeFactory _serviceScopeFactory;

        #endregion

        public AuthenticateService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        private readonly Dictionary<string, SessinoContext> _sessions =
            new Dictionary<string, SessinoContext>();

        public SessinoContext GetSessionInfo(string sessionToken)
        {
            throw new NotImplementedException();
        }

        public AuthenticationResponse Login(AuthenticationRequest authenticationRequest)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            ClinicServiceDbContext context = scope.ServiceProvider.GetRequiredService<ClinicServiceDbContext>();

            Account account =
                !string.IsNullOrWhiteSpace(authenticationRequest.Login) ?
                FindAccountByLogin(context, authenticationRequest.Login) : null;

            if (account == null)
            {
                return new AuthenticationResponse
                {
                    Status = AuthenticationStatus.UserNotFound
                };

            }


            if (!PasswordUtils.VerifyPassword(authenticationRequest.Password, account.PasswordSalt, account.PasswordHash))
            {
                return new AuthenticationResponse
                {
                    Status = AuthenticationStatus.InvalidPassword
                };
            }

            AccountSession session = new AccountSession
            {
                SessionToken = CreateSessionToken(account),
                AccountId = account.AccountId,
                TimeCreated = DateTime.Now,
                TimeLastRequest = DateTime.Now,
                IsClosed = false,
            };


            context.AccountSessions.Add(session);
            context.SaveChanges();

            SessinoContext sessionContext = GetSessionContext(account, session);

            lock (_sessions)
            {
                _sessions[sessionContext.SessionToken] = sessionContext;
            }

            return new AuthenticationResponse
            {
                Status = AuthenticationStatus.Success,
                SessionContext = sessionContext
            };

        }


        private SessinoContext GetSessionContext(Account account, AccountSession accountSession)
        {
            return new SessinoContext
            {
                SessionId = accountSession.SessionId,
                SessionToken = accountSession.SessionToken,
                Account = new AccountDto
                {
                    AccountId = account.AccountId,
                    EMail = account.EMail,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    SecondName = account.SecondName,
                    Locked = account.Locked
                }
            };
        }

        private string CreateSessionToken(Account account)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(SecretKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                        new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                        new Claim(ClaimTypes.Name, account.EMail),
                    }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        private Account FindAccountByLogin(ClinicServiceDbContext context, string login)
        {
            return context
                .Accounts
                .FirstOrDefault(account => account.EMail == login);
        }

    }
}
