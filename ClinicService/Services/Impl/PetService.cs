
using ClinicService.Data;
using Grpc.Core;
using PetServiceNamespace;

namespace ClinicService.Services.Impl;

public class PetService : PetServiceNamespace.PetService.PetServiceBase
{
    private readonly ClinicServiceDbContext _dbContext;

    public PetService(ClinicServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override Task<CreatePetResponse> CreatePet(CreatePetRequest request, ServerCallContext context)
    {
        try
        {
            var pet = new Pet
            {
                Name = request.Name,
                Birthday = request.Birthday,
                ClientId = request.ClientId
            };
            _dbContext.Pets.Add(pet);
            _dbContext.SaveChanges();

            var response = new CreatePetResponse
            {
                PetId = pet.Id,
                ErrCode = 0,
                ErrMessage = ""
            };

            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            var response = new CreatePetResponse
            {
                ErrCode = 1001,
                ErrMessage = "Internal server error."
            };

            return Task.FromResult(response);
        }
    }

    public override Task<GetPetsResponse> GetPets(GetPetsRequest request, ServerCallContext context)
    {
        try
        {
            var response = new GetPetsResponse();
            var pets = _dbContext.Pets.Select(pet => new PetResponse
            {
                PetId = pet.Id,
                Name = pet.Name,
                Birthday = pet.Birthday
            }).ToList();
            response.Pets.AddRange(pets);
            return Task.FromResult(response);
        }
        catch (Exception e)
        {
            var response = new GetPetsResponse
            {
                ErrCode = 1002,
                ErrMessage = "Internal server error."
            };

            return Task.FromResult(response);
        }
    }
}