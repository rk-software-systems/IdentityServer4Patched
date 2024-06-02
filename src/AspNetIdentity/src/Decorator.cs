// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdentityServer4.AspNetIdentity;

internal class Decorator<TService>(TService instance)
{
    public TService Instance { get; set; } = instance;
}

internal class Decorator<TService, TImpl>(TImpl instance) : Decorator<TService>(instance)
    where TImpl : class, TService
{
}

internal class DisposableDecorator<TService>(TService instance) : Decorator<TService>(instance), IDisposable
{
    public void Dispose()
    {
        (Instance as IDisposable)?.Dispose();
    }
}
