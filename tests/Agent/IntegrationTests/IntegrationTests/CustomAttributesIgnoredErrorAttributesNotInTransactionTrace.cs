﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using NewRelic.Agent.IntegrationTestHelpers;
using NewRelic.Agent.IntegrationTestHelpers.Models;
using NewRelic.Agent.IntegrationTests.RemoteServiceFixtures;
using NewRelic.Testing.Assertions;
using Xunit;
using Xunit.Abstractions;

namespace NewRelic.Agent.IntegrationTests
{
    public class CustomAttributesIgnoredErrorAttributesNotInTransactionTrace : IClassFixture<CustomAttributesWebApi>
    {
        [NotNull]
        private readonly CustomAttributesWebApi _fixture;

        public CustomAttributesIgnoredErrorAttributesNotInTransactionTrace([NotNull] CustomAttributesWebApi fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _fixture.TestLogger = output;
            _fixture.Actions
            (
                setupConfiguration: () =>
                {
                    var configPath = fixture.DestinationNewRelicConfigFilePath;
                    var configModifier = new NewRelicConfigModifier(configPath);
                    configModifier.ForceTransactionTraces();

                    CommonUtils.ModifyOrCreateXmlAttributeInNewRelicConfig(configPath, new[] { "configuration", "log" }, "level", "debug");
                    CommonUtils.ModifyOrCreateXmlNodeInNewRelicConfig(configPath, new[] { "configuration", "errorCollector", "ignoreErrors" }, "exception", "Custom Error");
                },
                exerciseApplication: () =>
                {
                    _fixture.Get404();
                    _fixture.AgentLog.WaitForLogLine(AgentLogFile.TransactionSampleLogLineRegex, TimeSpan.FromMinutes(2));
                }
            );
            _fixture.Initialize();
        }

        [Fact]
        public void Test()
        {
            var unexpectedTransactionTraceAttributes = new List<String>
            {
                "key",
                "foo",
                "hey",
                "faz",
            };
            var unexpectedTranscationEventAttributes = new List<String>
            {
                "key",
                "foo",
                "hey",
                "faz",
            };

            var transactionSample = _fixture.AgentLog.GetTransactionSamples().FirstOrDefault();
            var errorTrace = _fixture.AgentLog.GetErrorTraces().FirstOrDefault();
            var transactionEvent = _fixture.AgentLog.GetTransactionEvents().FirstOrDefault();

            NrAssert.Multiple
            (
                () => Assert.NotNull(transactionSample),
                () => Assert.NotNull(transactionEvent)
            );

            NrAssert.Multiple
            (
                () => Assert.Null(errorTrace),
                () => Assertions.TransactionTraceDoesNotHaveAttributes(unexpectedTransactionTraceAttributes, TransactionTraceAttributeType.User, transactionSample),
                () => Assertions.TransactionEventDoesNotHaveAttributes(unexpectedTranscationEventAttributes, TransactionEventAttributeType.User, transactionEvent)
            );
        }
    }
}
