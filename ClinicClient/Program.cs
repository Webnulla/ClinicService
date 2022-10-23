using ClinicServiceNamespace;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using PetServiceNamespace;

namespace ClinicClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            using var channel = GrpcChannel.ForAddress("http://localhost:5001");
            ClinicService.ClinicServiceClient clinicServiceClient = new ClinicService.ClinicServiceClient(channel);
            PetService.PetServiceClient petServiceClient = new PetService.PetServiceClient(channel);

            var createClientResponse = clinicServiceClient.CreateClient(new CreateClientRequest
            {
                Document = "DOC333",
                Firstname = "Star",
                Patronymic = "Jason",
                Surname = "Lord"
            });

            if (createClientResponse.ErrCode == 0)
            {
                Console.WriteLine($"Client #{createClientResponse.ClientId} created successfully.");
            }
            else
            {
                Console.WriteLine(
                    $"Create client error.\nErrorCode: {createClientResponse.ErrCode}\nErrorMessage: {createClientResponse.ErrMessage}");
            }

            var getClientResponse = clinicServiceClient.GetClients(new GetClientsRequest());

            if (createClientResponse.ErrCode == 0)
            {
                Console.WriteLine("Clients");
                Console.WriteLine("=======\n");

                foreach (var client in getClientResponse.Clients)
                {
                    Console.WriteLine($"#{client.ClientId} {client.Document} {client.Surname} {client.Firstname} {client.Patronymic}");
                }
            }
            else
            {
                Console.WriteLine($"Get clients error.\nErrorCode: {getClientResponse.ErrCode}\nErrorMessage: {getClientResponse.ErrMessage}");
            }

            Console.WriteLine("-------------------------------------------------------------------");

            var createPetResponse = petServiceClient.CreatePet(new CreatePetRequest
            {
                Name = "Unicorn",
                Birthday = Timestamp.FromDateTime(DateTime.UtcNow),
                ClientId = 1
            });
            
            if (createPetResponse.ErrCode == 0)
            {
                Console.WriteLine($"Client #{createPetResponse.PetId} created successfully.");
            }
            else
            {
                Console.WriteLine(
                    $"Create client error.\nErrorCode: {createPetResponse.ErrCode}\nErrorMessage: {createPetResponse.ErrMessage}");
            }
                        
            var getPetResponse = petServiceClient.GetPets(new GetPetsRequest());

            if (createPetResponse.ErrCode == 0)
            {
                Console.WriteLine("Pets");
                Console.WriteLine("=======\n");

                foreach (var pet in getPetResponse.Pets)
                {
                    Console.WriteLine($"#{pet.Name} {pet.Birthday} {pet.PetId}");
                }
            }
            else
            {
                Console.WriteLine($"Get pets error.\nErrorCode: {getPetResponse.ErrCode}\nErrorMessage: {getPetResponse.ErrMessage}");
            }
            
            Console.ReadKey();
        }
    }
}