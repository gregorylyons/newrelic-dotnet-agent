﻿using System;
using System.Collections.Generic;
using System.Linq;
using AttributeFilterTests.Models;
using JetBrains.Annotations;
using NewRelic.Agent;
using Newtonsoft.Json;
using NUnit.Framework;
using Attribute = AttributeFilterTests.Models.Attribute;

namespace AttributeFilterTests
{
    public class AttributeFilterTests
    {
        private static String TestCaseData
        {
            get
            {
                return
@"[
  {
    ""testname"":""everything enabled, no include/exclude"",
    ""config"":
    {
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"", ""browser_monitoring""],
    ""expected_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""]
  },

  {
    ""testname"":""browser monitoring attributes disabled by default"",
    ""config"":
    {
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"", ""browser_monitoring""],
    ""expected_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector""]
  },

  {
    ""testname"":""attributes globally disabled"",
    ""config"":
    {
      ""attributes.enabled"":false
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""all categories disabled"",
    ""config"":
    {
      ""transaction_events.attributes.enabled"":false,
      ""transaction_tracer.attributes.enabled"":false,
      ""error_collector.attributes.enabled"":false
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""global exclude"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""exclude in each category"",
    ""config"":
    {
      ""transaction_events.attributes.exclude"":[""alpha""],
      ""transaction_tracer.attributes.exclude"":[""alpha""],
      ""error_collector.attributes.exclude"":[""alpha""],
      ""browser_monitoring.attributes.enabled"":true,
      ""browser_monitoring.attributes.exclude"":[""alpha""]
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""global include"",
    ""config"":
    {
      ""attributes.include"":[""alpha""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [],
    ""expected_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""]
  },

  {
    ""testname"":""each category include"",
    ""config"":
    {
      ""transaction_events.attributes.include"":[""alpha""],
      ""transaction_tracer.attributes.include"":[""alpha""],
      ""error_collector.attributes.include"":[""alpha""],
      ""browser_monitoring.attributes.enabled"":true,
      ""browser_monitoring.attributes.include"":[""alpha""]
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [],
    ""expected_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""]
  },

  {
    ""testname"":""global include/exclude contradict"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha""],
      ""attributes.include"":[""alpha""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""include/exclude contradict in each category"",
    ""config"":
    {
      ""transaction_events.attributes.exclude"":[""alpha""],
      ""transaction_events.attributes.include"":[""alpha""],
      ""transaction_tracer.attributes.exclude"":[""alpha""],
      ""transaction_tracer.attributes.include"":[""alpha""],
      ""error_collector.attributes.exclude"":[""alpha""],
      ""error_collector.attributes.include"":[""alpha""],
      ""browser_monitoring.attributes.enabled"":true,
      ""browser_monitoring.attributes.exclude"":[""alpha""],
      ""browser_monitoring.attributes.include"":[""alpha""]
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""global exclude contradicts category include"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha""],
      ""transaction_events.attributes.include"":[""alpha""],
      ""transaction_tracer.attributes.include"":[""alpha""],
      ""error_collector.attributes.include"":[""alpha""],
      ""browser_monitoring.attributes.enabled"":true,
      ""browser_monitoring.attributes.include"":[""alpha""]
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""global include contradicts category exclude"",
    ""config"":
    {
      ""attributes.include"":[""alpha""],
      ""transaction_events.attributes.exclude"":[""alpha""],
      ""transaction_tracer.attributes.exclude"":[""alpha""],
      ""error_collector.attributes.exclude"":[""alpha""],
      ""browser_monitoring.attributes.enabled"":true,
      ""browser_monitoring.attributes.exclude"":[""alpha""]
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""alpha is more specific than alpha*"",
    ""config"":
    {
      ""attributes.include"":[""alpha""],
      ""transaction_events.attributes.exclude"":[""alpha*""],
      ""transaction_tracer.attributes.exclude"":[""alpha*""],
      ""error_collector.attributes.exclude"":[""alpha*""],
      ""browser_monitoring.attributes.enabled"":true,
      ""browser_monitoring.attributes.exclude"":[""alpha*""]
    },
    ""input_key"":""alpha"",
    ""input_default_destinations"":
      [],
    ""expected_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""]
  },

  {
    ""testname"":""all destination modifiers applied, not only the most specific one"",
    ""config"":
    {
      ""attributes.exclude"":[""a*""],
      ""transaction_events.attributes.include"":[""ab*""],
      ""transaction_events.attributes.exclude"":[""abc*""],
      ""transaction_tracer.attributes.exclude"":[""abcd*""],
      ""transaction_tracer.attributes.include"":[""abcde*""],
      ""error_collector.attributes.include"":   [""abcdef*""],
      ""error_collector.attributes.exclude"":   [""abcdefg*""],
      ""browser_monitoring.attributes.exclude"":[""abcdefgh*""],
      ""browser_monitoring.attributes.include"":[""abcdefghi*""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""abcdefghik"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      [""transaction_tracer"",""browser_monitoring""]
  },


  {
    ""testname"":""venn diagram part 1"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha.*"", ""alpha.beta.gamma.*""],
      ""attributes.include"":[""alpha.beta.*""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha."",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""venn diagram part 2"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha.*"", ""alpha.beta.gamma.*""],
      ""attributes.include"":[""alpha.beta.*""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha.psi"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""venn diagram part 3"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha.*"", ""alpha.beta.gamma.*""],
      ""attributes.include"":[""alpha.beta.*""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha.beta."",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""]
  },

  {
    ""testname"":""venn diagram part 4"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha.*"", ""alpha.beta.gamma.*""],
      ""attributes.include"":[""alpha.beta.*""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha.beta.psi"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""]
  },

  {
    ""testname"":""venn diagram part 5"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha.*"", ""alpha.beta.gamma.*""],
      ""attributes.include"":[""alpha.beta.*""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha.beta.gamma."",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""alpha is not mistaken for alpha*"",
    ""config"":
    {
      ""transaction_events.attributes.include"":[""alpha""],
      ""transaction_tracer.attributes.exclude"":[""alpha*""],
      ""error_collector.attributes.include"":   [""alpha""],
      ""browser_monitoring.attributes.exclude"":[""alpha*""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""alpha.beta"",
    ""input_default_destinations"":
      [""transaction_tracer"",""browser_monitoring""],
    ""expected_destinations"":
      []
  },

  {
    ""testname"":""exact match is case sensitive"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""ALPHA"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""]
  },

  {
    ""testname"":""wildcard match is case sensitive"",
    ""config"":
    {
      ""attributes.exclude"":[""alpha.*""],
      ""browser_monitoring.attributes.enabled"":true
    },
    ""input_key"":""ALPHA.BETA"",
    ""input_default_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""],
    ""expected_destinations"":
      [""transaction_events"", ""transaction_tracer"", ""error_collector"",""browser_monitoring""]
  }
]";
            }
        }

        public static IEnumerable<TestCase[]> TestCases
        {
            get
            {
                return JsonConvert.DeserializeObject<IEnumerable<TestCase>>(TestCaseData)
                    .Select(testCase => new[] { testCase });
            }
        }

        [TestCaseSource(typeof(AttributeFilterTests), "TestCases")]
        public void when([NotNull] TestCase testCase)
        {
            // Arrange
            var attributeFilterSettings = testCase.Configuration.ToAttributeFilterSettings();
            var attributeFilter = new AttributeFilter<Attribute>(attributeFilterSettings);
            var attribute = new Attribute(testCase.AttributeDestinations.ToAttributeDestination(), testCase.AttributeKey, "foo");

            // Act
            var errorDestination = attributeFilter.FilterAttributes(new[] { attribute }, AttributeDestinations.ErrorTrace);
            var javaScriptDestination = attributeFilter.FilterAttributes(new[] { attribute }, AttributeDestinations.JavaScriptAgent);
            var transactionEventDestination = attributeFilter.FilterAttributes(new[] { attribute }, AttributeDestinations.TransactionEvent);
            var transactionTraceDestination = attributeFilter.FilterAttributes(new[] { attribute }, AttributeDestinations.TransactionTrace);

            // Assert
            if (testCase.ExpectedDestinations.Contains(Destinations.BrowserMonitoring))
                Assert.True(javaScriptDestination.Any(), testCase.TestName);
            else
                Assert.False(javaScriptDestination.Any(), testCase.TestName);

            if (testCase.ExpectedDestinations.Contains(Destinations.ErrorCollector))
                Assert.True(errorDestination.Any(), testCase.TestName);
            else
                Assert.False(errorDestination.Any(), testCase.TestName);

            if (testCase.ExpectedDestinations.Contains(Destinations.TransactionEvents))
                Assert.True(transactionEventDestination.Any(), testCase.TestName);
            else
                Assert.False(transactionEventDestination.Any(), testCase.TestName);

            if (testCase.ExpectedDestinations.Contains(Destinations.TransactionTracer))
                Assert.True(transactionTraceDestination.Any(), testCase.TestName);
            else
                Assert.False(transactionTraceDestination.Any(), testCase.TestName);
        }
    }
}
