using ClinicService.Data;
using ClinicServiceNamespace;
using Grpc.Core;

namespace ClinicService.Services.Impl
{
    public class ClinicClientService : ClinicServiceNamespace.ClinicService.ClinicServiceBase
    {
        private readonly ClinicServiceDbContext _dbContext;
        private readonly ILogger<ClinicClientService> _logger;


        public ClinicClientService(ClinicServiceDbContext dbContext,
            ILogger<ClinicClientService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override Task<CreateClientResponse> CreateClient(CreateClientRequest request, ServerCallContext context)
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
                ClientId = client.Id
            };

            return Task.FromResult(response);
        }

        public override Task<GetClientsResponse> GetClients(GetClientsRequest request, ServerCallContext context)
        {
            var response = new GetClientsResponse();

            response.Clients.AddRange(_dbContext.Clients.Select(client => new ClientResponse
            {
                ClientId = client.Id,
                Document = client.Document,
                Firstname = client.FirstName,
                Patronymic = client.Patronymic,
                Surname = client.Surname
            }).ToList());

            return Task.FromResult(response);
        }
    }
}
