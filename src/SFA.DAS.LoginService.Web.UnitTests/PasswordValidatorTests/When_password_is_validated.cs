using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.Web.Infrastructure;
using SFA.DAS.LoginService.Web.UnitTests.Controllers.PasswordReset;

namespace SFA.DAS.LoginService.Web.UnitTests.PasswordValidatorTests
{
    [TestFixture]
    public class When_password_is_validated
    {
        [TestCase("11aaa222", true)]
        [TestCase(" 11AAA 222 ", true)]
        [TestCase("11AAA222!!", true)]
        [TestCase("11aa333", false)]
        [TestCase("        ", false)]
        [TestCase("11553333", false)]
        [TestCase("abcDEFGH", false)]
        [TestCase("qwertyu1", false)]
        public async Task Then_password_is_validated_correctly(string newpassword, bool expectedValidity)
        {           
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var _loginContext = new LoginContext(dbContextOptions);

            _loginContext.InvalidPasswords.Add(new InvalidPassword() {Password = "pa55word"});
            _loginContext.InvalidPasswords.Add(new InvalidPassword() {Password = "qwertyu1"});
            _loginContext.InvalidPasswords.Add(new InvalidPassword() {Password = "l3tm31n!"});
            await _loginContext.SaveChangesAsync();

            var validator = new CustomPasswordValidator<LoginUser>(Substitute.For<ILogger<CustomPasswordValidator<LoginUser>>>(), _loginContext);

            var userManager = new UserManager<LoginUser>(Substitute.For<IUserStore<LoginUser>>(), Substitute.For<IOptions<IdentityOptions>>(), Substitute.For<IPasswordHasher<LoginUser>>()
                , Substitute.For<IEnumerable<IUserValidator<LoginUser>>>(), new List<IPasswordValidator<LoginUser>>(), Substitute.For<ILookupNormalizer>(), new IdentityErrorDescriber(), Substitute.For<IServiceProvider>(),
                Substitute.For<ILogger<UserManager<LoginUser>>>());
            
            var result = await validator.ValidateAsync(userManager, new LoginUser(), newpassword);

            (result == IdentityResult.Success).Should().Be(expectedValidity);
        }
    }
}