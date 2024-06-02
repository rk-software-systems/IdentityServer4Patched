// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Stores;

namespace IdentityServer4.Extensions
{
    /// <summary>
    /// Extensions for PersistedGrantFilter.
    /// </summary>
    public static class PersistedGrantFilterExtensions
    {
        /// <summary>
        /// Validates the PersistedGrantFilter and throws if invalid.
        /// </summary>
        /// <param name="filter"></param>
        public static void Validate(this PersistedGrantFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter, nameof(filter));

            if (string.IsNullOrWhiteSpace(filter.ClientId) &&
                string.IsNullOrWhiteSpace(filter.SessionId) &&
                string.IsNullOrWhiteSpace(filter.SubjectId) &&
                string.IsNullOrWhiteSpace(filter.Type))
            {
                throw new ArgumentException("No filter values set.", nameof(filter));
            }
        }
    }
}