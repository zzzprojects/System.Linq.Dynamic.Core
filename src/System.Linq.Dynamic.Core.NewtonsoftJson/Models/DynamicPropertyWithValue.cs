﻿namespace System.Linq.Dynamic.Core.NewtonsoftJson.Models;

internal class DynamicPropertyWithValue : DynamicProperty
{
    public object? Value { get; }

    public DynamicPropertyWithValue(string name, object? value) : base(name, value?.GetType() ?? typeof(object))
    {
        Value = value;
    }
}