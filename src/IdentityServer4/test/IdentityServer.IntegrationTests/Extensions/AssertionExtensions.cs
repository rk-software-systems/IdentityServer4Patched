using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace IdentityServer.IntegrationTests;

public static class AssertionExtensions
{    
    public static void ShouldContain(this IDictionary<string, JsonNode> dictionary, string key, string value)
    {
        dictionary.Should().Contain(x => x.Key.Equals(key, StringComparison.Ordinal) && x.Value.ToString().Equals(value, StringComparison.Ordinal));
    }

    public static void ShouldBe(this JsonNode node, string value)
    {
        node.ToString().Should().Be(value);
    }
}
