using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Moq;
using SSW.TimePRO.AutoTimeSheeting.Infrastructure.ClientRate;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SSW.TimePRO.AutoTimeSheeting.Infrastructure.Tests.Integration
{
    public class ClientRateTests
    {
        private readonly ILogger _logger = NullLoggerFactory.Instance.CreateLogger("test");

        [Fact]
        public async Task ShouldBeValid()
        {
            var mockQuery = new Mock<IGetClientRateQuery>();
            mockQuery
                .Setup(x => x.Execute(It.IsAny<GetClientRateRequest>()))
                .ReturnsAsync(new ClientRateModel
                {
                    ClientEmpRate = 245m,
                    ClientTaxRate = 0.1m
                });

            var request = new DefaultHttpRequest(new DefaultHttpContext());
            var queryParam = new Dictionary<string, StringValues>
            {
                { "tenantUrl", "http://asdasd.ayz" },
                { "empID", "JEK" },
                { "clientID", "SSW" },
                { "token", "test-token" }
            };
            request.Query = new QueryCollection(queryParam);
            var response = await AzureFunctions.ClientRate.Run(request, _logger, mockQuery.Object);

            response.Should().BeOfType<JsonResult>();
            var result = ((JsonResult)response).Value as ClientRateModel;

            result.Should().NotBeNull();
            result.ClientEmpRate.Should().Be(245m);
            result.ClientTaxRate.Should().Be(0.1m);
        }

        [Fact]
        public async Task ShouldFail()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());
            var response = await AzureFunctions.ClientRate.Run(request, _logger, null);

            response.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
