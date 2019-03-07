using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.ResetPassword;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Application.UnitTests.ResetPassword
{
    public class When_ResetPasswordCode_called_for_nonexistant_account : ResetPasswordCodeTestBase
    {
        [Test]
        public async Task Then_a_reset_password_email_is_not_sent()
        {
            UserService.FindByEmail(Arg.Any<string>()).Returns(default(LoginUser));
            
            await Handler.Handle(new ResetPasswordCodeRequest() {Email = "nonexistantuser@emailaddress.com", ClientId = ClientId}, CancellationToken.None);
            
            await EmailService.DidNotReceive().SendResetPassword("nonexistantuser@emailaddress.com", Arg.Any<string>(), Arg.Any<string>() );
        }
    }
}