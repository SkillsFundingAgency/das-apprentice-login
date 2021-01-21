using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace SFA.DAS.LoginService.Web.IntegrationTests.SignUpComplete
{
    [TestFixture]
    public class When_SignUpComplete_endpoint_GET
    {
        [Test]
        public async Task Then_404_NotFound_is_not_returned()
        {
            var client = new CustomWebApplicationFactory<Startup>().CreateClient();

            var response = await client.GetAsync("/Invitations/SignUpComplete/" + Guid.NewGuid());

            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }
    }
}