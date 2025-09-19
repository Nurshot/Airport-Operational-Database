using AODB.Application.Common.Behaviours;
using AODB.Application.Common.Interfaces;

using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace AODB.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
   
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
       
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

   
 
}
