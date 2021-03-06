using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.Data.JsonObjects;

namespace SFA.DAS.LoginService.Application.UnitTests.Services.ClientServiceTests
{
    [TestFixture]
    public class GetClientById_called_for_valid_clientId
    {
        [Test]
        public async Task Then_correct_Client_is_returned()
        {
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: "GetClientByIdHandler_tests")
                .Options;

            var loginContext = new LoginContext(dbContextOptions);

            var clientId = Guid.NewGuid();
            loginContext.Clients.AddRange(new List<Client>
            {
                new Client(){Id = Guid.NewGuid(), ServiceDetails = new ServiceDetails{ServiceName = "Service 1", SupportUrl = "https://support/Url/1"}},
                new Client(){Id = clientId, ServiceDetails = new ServiceDetails{ServiceName = "Service 2", SupportUrl = "https://support/Url/2"}},
                new Client(){Id = Guid.NewGuid(), ServiceDetails = new ServiceDetails{ServiceName = "Service 3", SupportUrl = "https://support/Url/3"}},
            });
            await loginContext.SaveChangesAsync();

            var interactionService = Substitute.For<IIdentityServerInteractionService>();
            var clientService = new ClientService(interactionService, loginContext);

            var clientResult = await clientService.GetByClientId(clientId, CancellationToken.None);

            clientResult.Id.Should().Be(clientId);
            clientResult.ServiceDetails.ServiceName.Should().Be("Service 2");
            clientResult.ServiceDetails.SupportUrl.Should().Be("https://support/Url/2");
        }
    }
}