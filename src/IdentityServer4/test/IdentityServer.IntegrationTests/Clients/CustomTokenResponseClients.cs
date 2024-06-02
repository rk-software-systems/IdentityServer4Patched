// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer.IntegrationTests.Clients.Setup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace IdentityServer.IntegrationTests.Clients;

public class CustomTokenResponseClients
{
    private const string TokenEndpoint = "https://server/connect/token";

    private readonly HttpClient _client;

    public CustomTokenResponseClients()
    {
        var builder = new WebHostBuilder()
            .UseStartup<StartupWithCustomTokenResponses>();
        var server = new TestServer(builder);

        _client = server.CreateClient();
    }

    [Fact]
    public async Task Resource_owner_success_should_return_custom_response()
    {
        var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = TokenEndpoint,
            ClientId = "roclient",
            ClientSecret = "secret",

            UserName = "bob",
            Password = "bob",
            Scope = "api1"
        });

        // raw fields
        var fields = response.GetFields();
        fields.ShouldContain("string_value", "some_string");
        
        fields["int_value"].GetValue<Int64>().Should().Be(42);

        fields.TryGetValue("identity_token", out var _).Should().BeFalse();
        fields.TryGetValue("refresh_token", out var _).Should().BeFalse();
        fields.TryGetValue("error", out var _).Should().BeFalse();
        fields.TryGetValue("error_description", out var _).Should().BeFalse();
        fields.TryGetValue("token_type", out var _).Should().BeTrue();
        fields.TryGetValue("expires_in", out var _).Should().BeTrue();

        var responseObject = fields["dto"];
        responseObject.Should().NotBeNull();

        var responseDto = GetDto(responseObject);
        var dto = CustomResponseDto.Create;

        responseDto.string_value.Should().Be(dto.string_value);
        responseDto.int_value.Should().Be(dto.int_value);
        responseDto.nested.string_value.Should().Be(dto.nested.string_value);
        responseDto.nested.int_value.Should().Be(dto.nested.int_value);


        // token client response
        response.IsError.Should().Be(false);
        response.ExpiresIn.Should().Be(3600);
        response.TokenType.Should().Be("Bearer");
        response.IdentityToken.Should().BeNull();
        response.RefreshToken.Should().BeNull();


        // token content
        var payload = response.GetPayload();
        payload.Count().Should().Be(12);

        payload.ShouldContain("iss", "https://idsvr4");
        payload.ShouldContain("client_id", "roclient");
        payload.ShouldContain("sub", "bob");
        payload.ShouldContain("idp", "local");

        payload["aud"].ShouldBe("api");

        var scopes = payload["scope"].AsArray();
        scopes.First().ToString().Should().Be("api1");

        var amr = payload["amr"].AsArray();
        amr.Count().Should().Be(1);
        amr.First().ToString().Should().Be("password");
    }

    [Fact]
    public async Task Resource_owner_failure_should_return_custom_error_response()
    {
        var response = await _client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = TokenEndpoint,
            ClientId = "roclient",
            ClientSecret = "secret",

            UserName = "bob",
            Password = "invalid",
            Scope = "api1"
        });

        // raw fields
        var fields = response.GetFields();

        fields.ShouldContain("string_value", "some_string");

        fields["int_value"].GetValue<Int64>().Should().Be(42);

        fields.TryGetValue("identity_token", out var _).Should().BeFalse();
        fields.TryGetValue("refresh_token", out var _).Should().BeFalse();
        fields.TryGetValue("error", out var _).Should().BeTrue();
        fields.TryGetValue("error_description", out var _).Should().BeTrue();
        fields.TryGetValue("token_type", out var _).Should().BeFalse();
        fields.TryGetValue("expires_in", out var _).Should().BeFalse();

        var responseObject = fields["dto"];
        responseObject.Should().NotBeNull();

        var responseDto = GetDto(responseObject);
        var dto = CustomResponseDto.Create;

        responseDto.string_value.Should().Be(dto.string_value);
        responseDto.int_value.Should().Be(dto.int_value);
        responseDto.nested.string_value.Should().Be(dto.nested.string_value);
        responseDto.nested.int_value.Should().Be(dto.nested.int_value);


        // token client response
        response.IsError.Should().Be(true);
        response.Error.Should().Be("invalid_grant");
        response.ErrorDescription.Should().Be("invalid_credential");
        response.ExpiresIn.Should().Be(0);
        response.TokenType.Should().BeNull();
        response.IdentityToken.Should().BeNull();
        response.RefreshToken.Should().BeNull();
    }

    [Fact]
    public async Task Extension_grant_success_should_return_custom_response()
    {
        var response = await _client.RequestTokenAsync(new TokenRequest
        {
            Address = TokenEndpoint,
            GrantType = "custom",

            ClientId = "client.custom",
            ClientSecret = "secret",

            Parameters =
            {
                { "scope", "api1" },
                { "outcome", "succeed"}
            }
        });


        // raw fields
        var fields = response.GetFields();

        fields.ShouldContain("string_value", "some_string");

        fields["int_value"].GetValue<Int64>().Should().Be(42);

        fields.TryGetValue("identity_token", out var _).Should().BeFalse();
        fields.TryGetValue("refresh_token", out var _).Should().BeFalse();
        fields.TryGetValue("error", out var _).Should().BeFalse();
        fields.TryGetValue("error_description", out var _).Should().BeFalse();
        fields.TryGetValue("token_type", out var _).Should().BeTrue();
        fields.TryGetValue("expires_in", out var _).Should().BeTrue();

        var responseObject = fields["dto"];
        responseObject.Should().NotBeNull();

        var responseDto = GetDto(responseObject);
        var dto = CustomResponseDto.Create;

        responseDto.string_value.Should().Be(dto.string_value);
        responseDto.int_value.Should().Be(dto.int_value);
        responseDto.nested.string_value.Should().Be(dto.nested.string_value);
        responseDto.nested.int_value.Should().Be(dto.nested.int_value);


        // token client response
        response.IsError.Should().Be(false);
        response.ExpiresIn.Should().Be(3600);
        response.TokenType.Should().Be("Bearer");
        response.IdentityToken.Should().BeNull();
        response.RefreshToken.Should().BeNull();


        // token content
        var payload = response.GetPayload();
        payload.Count().Should().Be(12);

        payload.ShouldContain("iss", "https://idsvr4");
        payload.ShouldContain("client_id", "client.custom");
        payload.ShouldContain("sub", "bob");
        payload.ShouldContain("idp", "local");

        payload["aud"].ShouldBe("api");

        var scopes = payload["scope"].AsArray();
        scopes.First().ToString().Should().Be("api1");

        var amr = payload["amr"].AsArray();
        amr.Count().Should().Be(1);
        amr.First().ToString().Should().Be("custom");

    }

    [Fact]
    public async Task Extension_grant_failure_should_return_custom_error_response()
    {
        var response = await _client.RequestTokenAsync(new TokenRequest
        {
            Address = TokenEndpoint,
            GrantType = "custom",

            ClientId = "client.custom",
            ClientSecret = "secret",

            Parameters =
            {
                { "scope", "api1" },
                { "outcome", "fail"}
            }
        });


        // raw fields
        var fields = response.GetFields();

        fields.ShouldContain("string_value", "some_string");

        fields["int_value"].GetValue<Int64>().Should().Be(42);

        fields.TryGetValue("identity_token", out var _).Should().BeFalse();
        fields.TryGetValue("refresh_token", out var _).Should().BeFalse();
        fields.TryGetValue("error", out var _).Should().BeTrue();
        fields.TryGetValue("error_description", out var _).Should().BeTrue();
        fields.TryGetValue("token_type", out var _).Should().BeFalse();
        fields.TryGetValue("expires_in", out var _).Should().BeFalse();

        var responseObject = fields["dto"];
        responseObject.Should().NotBeNull();

        var responseDto = GetDto(responseObject);
        var dto = CustomResponseDto.Create;

        responseDto.string_value.Should().Be(dto.string_value);
        responseDto.int_value.Should().Be(dto.int_value);
        responseDto.nested.string_value.Should().Be(dto.nested.string_value);
        responseDto.nested.int_value.Should().Be(dto.nested.int_value);


        // token client response
        response.IsError.Should().Be(true);
        response.Error.Should().Be("invalid_grant");
        response.ErrorDescription.Should().Be("invalid_credential");
        response.ExpiresIn.Should().Be(0);
        response.TokenType.Should().BeNull();
        response.IdentityToken.Should().BeNull();
        response.RefreshToken.Should().BeNull();
    }

    private static CustomResponseDto GetDto(JsonNode responseObject)
    {
        return JsonSerializer.Deserialize<CustomResponseDto>(responseObject);
    }
}