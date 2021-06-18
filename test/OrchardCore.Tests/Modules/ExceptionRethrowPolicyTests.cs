using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using OrchardCore.Modules;
using OrchardCore.Tests.Stubs;
using Xunit;

namespace OrchardCore.Tests.Modules
{
    public class ExceptionRethrowPolicyTests
    {
        private ExceptionRethrowPolicy<ExceptionRethrowPolicyTests> GetRethrowPolicy(ExceptionRethrowOptions rethrowOptions)
        {
            var options = new Mock<IOptions<ExceptionRethrowOptions>>();
            options.Setup(a => a.Value).Returns(rethrowOptions);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.Request.Path = new PathString("/abc");
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            return new ExceptionRethrowPolicy<ExceptionRethrowPolicyTests>(mockHttpContextAccessor.Object, options.Object);

        }


        [Fact]
        public void NotPolicySetShouldReturnFalse()
        {
            var options = new ExceptionRethrowOptions();
            var policy = GetRethrowPolicy(options);

            Assert.False(policy.ShouldThrow(new Exception()));
        }

        [Fact]
        public void RethrowAllShouldReturnTrueAlways()
        {
            var options = new ExceptionRethrowOptions();
            options.RethrowAll();

            var policy = GetRethrowPolicy(options);

            Assert.True(policy.ShouldThrow(new Exception()));
        }

        public static IEnumerable<object[]> NonFatalExceptions => new List<object[]> {
            new object[] { new Exception() },
            new object[] { new InvalidOperationException() },
            new object[] { new ArgumentException() },
            new object[] { new ArgumentOutOfRangeException() }
        };

        [Theory]
        [MemberData(nameof(NonFatalExceptions))]
        public void RethrowFatalShouldReturnFalseForNonFatalExceptions(Exception ex)
        {
            var options = new ExceptionRethrowOptions();
            options.RethrowFatal();

            var policy = GetRethrowPolicy(options);

            Assert.False(policy.ShouldThrow(ex));
        }

        public static IEnumerable<object[]> FatalExceptions => new List<object[]> {
            new object[] { new OutOfMemoryException() },
            new object[] { new SecurityException() },
            new object[] { new SEHException() }
        };

        [Theory]
        [MemberData(nameof(FatalExceptions))]
        public void RethrowFatalShouldReturnTrueForFatalException(Exception ex)
        {
            var options = new ExceptionRethrowOptions();
            options.RethrowFatal();

            var policy = GetRethrowPolicy(options);

            Assert.True(policy.ShouldThrow(ex));
        }

        public class DumbyClass
        {

        }

        [Fact]
        public void TypedRethrowFuncShouldNotBeUsedIfCallingTypeDoesNotMatch()
        {
            var options = new ExceptionRethrowOptions();
            options.Rethrow<DumbyClass>((context, ex) => true);

            var policy = GetRethrowPolicy(options);

            Assert.False(policy.ShouldThrow(new Exception()));
        }

        [Fact]
        public void TypedRethrowFuncShouldBeUsedIfCallingTypeDoesMatch()
        {
            var options = new ExceptionRethrowOptions();
            options.Rethrow<ExceptionRethrowPolicyTests>((context, ex) => true);

            var policy = GetRethrowPolicy(options);

            Assert.True(policy.ShouldThrow(new Exception()));
        }

        [Fact]
        public void ShouldReturnTrueIfAnyRethrowFuncReturnTrue()
        {
            var options = new ExceptionRethrowOptions();

            options.Rethrow<ExceptionRethrowPolicyTests>((context, ex) => false);
            options.Rethrow<ExceptionRethrowPolicyTests>((context, ex) => true);
            options.Rethrow<ExceptionRethrowPolicyTests>((context, ex) => false);

            var policy = GetRethrowPolicy(options);

            Assert.True(policy.ShouldThrow(new Exception()));
        }
    }
}
