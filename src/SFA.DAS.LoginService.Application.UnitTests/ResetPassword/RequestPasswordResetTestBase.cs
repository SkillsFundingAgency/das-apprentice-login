using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.ResetPassword;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Application.UnitTests.ResetPassword
{
    [TestFixture]
    public class RequestPasswordResetTestBase
    {
        protected IEmailService EmailService;
        protected ICodeGenerationService CodeGenerationService;
        protected ILoginConfig LoginConfig;
        protected RequestPasswordResetHandler Handler;
        protected Guid ClientId;
        protected LoginContext LoginContext;
        protected IUserService UserService;
        protected IHashingService HashingService;

        [SetUp]
        public async Task SetUp()
        {
            ClientId = Guid.NewGuid();
            EmailService = Substitute.For<IEmailService>();
            CodeGenerationService = Substitute.For<ICodeGenerationService>();
            
            UserService = Substitute.For<IUserService>();
            
            LoginConfig = Substitute.For<ILoginConfig>();
            LoginConfig.BaseUrl.Returns("https://baseurl");
            LoginConfig.PasswordResetExpiryInHours = 1;
            
            HashingService = Substitute.For<IHashingService>();
            
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            LoginContext = new LoginContext(dbContextOptions);
            
            Handler = new RequestPasswordResetHandler(EmailService, CodeGenerationService, LoginConfig, LoginContext, UserService, HashingService);
        }
        
    }
}