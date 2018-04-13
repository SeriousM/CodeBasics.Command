// //  -----------------------------------------------------------------------
// //  <copyright file="TypeExtensions.cs" company="AXA France Service">
// //      Copyright (c) AXA France Service. All rights reserved.
// //  </copyright>
// //  <actor>S614599 (VANDENBUSSCHE Julien)</actor>
// //  <created>30/11/2016 14:52</created>
// //  <modified>16/01/2017 15:27</modified>
// //  -----------------------------------------------------------------------

namespace Command.Extensions
{
    using System;
    using System.Reflection;

    internal static class TypeExtensions
    {
        public static bool IsPrimitive(this Type that)
        {
            return that.GetTypeInfo().IsPrimitive || that == typeof(string) || that == typeof(decimal)
                   || that == typeof(DateTime);
        }
    }
}