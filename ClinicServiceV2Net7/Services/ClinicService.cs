using ClinicService.Data;
using ClinicServiceNamespace;
using Grpc.Core;

namespace ClinicServiceV2Net7.Services
{
    public class ClinicService : ClinicServiceNamespace.ClinicService.ClinicServiceBase
    {
        private readonly ClinicServiceDbContext _dbContext;

        public ClinicService(ClinicServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<CreateClientResponse> CreateClient(CreateClientRequest request, ServerCallContext context)
        {
            try
            {
                var client = new Client
                {
                    Document = request.Document,
                    Surname = request.Surname,
                    FirstName = request.Firstname,
                    Patronymic = request.Patronymic
                };
                _dbContext.Clients.Add(client);
                _dbContext.SaveChanges();

                var response = new CreateClientResponse
                {
                    ClientId = client.Id,
                    ErrCode = 0,
                    ErrMessage = ""
                };

                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                var response = new CreateClientResponse
                {
                    ErrCode = 1001,
                    ErrMessage = "Internal server error."
                };

                return Task.FromResult(response);
            }
        }

        public override Task<GetClientsResponse> GetClients(GetClientsRequest request, ServerCallContext context)
        {
            try
            {
                var response = new GetClientsResponse();
                var clients = _dbContext.Clients.Select(client => new ClientResponse
                {
                    ClientId = client.Id,
                    Document = client.Document,
                    Firstname = client.FirstName,
                    Surname = client.Surname,
                    Patronymic = client.Patronymic
                }).ToList();
                response.Clients.AddRange(clients);
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                var response = new GetClientsResponse
                {
                    ErrCode = 1002,
                    ErrMessage = "Internal server error."
                };

                return Task.FromResult(response);
            }
        }
    }
}
