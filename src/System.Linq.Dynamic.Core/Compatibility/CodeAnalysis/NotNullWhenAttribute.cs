#region License
// MIT License
// 
// Copyright (c) Manuel Römer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#if NETSTANDARD1_3_OR_GREATER || NET35 || NET40 || NET45 || NET452 || NET46 || NETCOREAPP2_1 || UAP10_0

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that when a method returns <see cref="ReturnValue"/>,
/// the parameter will not be <see langword="null"/> even if the corresponding type allows it.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[DebuggerNonUserCode]
internal sealed class NotNullWhenAttribute : Attribute
{
    /// <summary>
    /// Gets the return value condition.
    /// If the method returns this value, the associated parameter will not be <see langword="null"/>.
    /// </summary>
    public bool ReturnValue { get; }

    /// <summary>
    /// Initializes the attribute with the specified return value condition.
    /// </summary>
    /// <param name="returnValue">
    /// The return value condition.
    /// If the method returns this value, the associated parameter will not be <see langword="null"/>.
    /// </param>
    public NotNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }
}
#endif