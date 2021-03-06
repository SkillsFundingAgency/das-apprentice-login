using System;
using System.Security.Authentication.ExtendedProtection;
using System.Threading;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.GetInvitationById;
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.Web.Controllers.InvitationsWeb;

namespace SFA.DAS.LoginService.Web.UnitTests.Controllers.SignUpComplete
{
    [TestFixture]
    public class SignUpCompleteTestsBase
    {
        protected IMediator Mediator;
        protected SignUpCompleteController Controller;
        protected Guid InvitationId;
        protected Guid ClientId;
        protected string ServiceName;

        [SetUp]
        public void SetUp()
        {
            Mediator = Substitute.For<IMediator>();
            Controller = new SignUpCompleteController(Mediator);
            InvitationId = Guid.NewGuid();
            ClientId = Guid.NewGuid();
            ServiceName = "A Client Service";
        }

        protected void SetValidInvitationByIdRequest()
        {
            Mediator.Send(Arg.Any<GetInvitationByIdRequest>(), CancellationToken.None).Returns(
                new Invitation()
                    {
                        UserRedirectUri = new Uri("https://localhost/redirect"), 
                        IsUserCreated = true,
                        ClientId = ClientId
                    });
        }
    }
}