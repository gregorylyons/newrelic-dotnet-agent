﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NewRelic.Agent.IntegrationTestHelpers;
using NewRelic.Agent.IntegrationTestHelpers.RemoteServiceFixtures;
using NewRelic.Testing.Assertions;
using Xunit;
using Xunit.Abstractions;

namespace NewRelic.Agent.IntegrationTests
{
    public class ApiAppNameChangeTests : IClassFixture<RemoteServiceFixtures.ApiAppNameChangeFixture>
    {
        [NotNull]
        private readonly RemoteServiceFixtures.ApiAppNameChangeFixture _fixture;

        public ApiAppNameChangeTests([NotNull] RemoteServiceFixtures.ApiAppNameChangeFixture fixture, [NotNull] ITestOutputHelper output)
        {
            _fixture = fixture;
            _fixture.TestLogger = output;
            _fixture.Actions(
                setupConfiguration: () =>
                {
                    var configModifier = new NewRelicConfigModifier(_fixture.DestinationNewRelicConfigFilePath);

                    CommonUtils.ModifyOrCreateXmlAttributesInNewRelicConfig(_fixture.DestinationNewRelicConfigFilePath, new[] { "configuration", "service" }, new[] { new KeyValuePair<String, String>("autoStart", "false") });
                });
            _fixture.Initialize();
        }

        [Fact]
        public void Test()
        {
            var expectedLogLineRegexes = new[]
            {
                @".+ Your New Relic Application Name\(s\): AgentApi",
                @".+ Your New Relic Application Name\(s\): AgentApi2"
            };
            var unexpectedLogLineRegexes = new[]
            {
                @".+ Your New Relic Application Name\(s\): " + RemoteApplication.AppName
            };

            var actualLogLines = _fixture.AgentLog.GetFileLines();

            NrAssert.Multiple
            (
                () => Assertions.LogLinesExist(expectedLogLineRegexes, actualLogLines),
                () => Assertions.LogLinesNotExist(unexpectedLogLineRegexes, actualLogLines)
            );
        }
    }
}
