﻿
namespace System.Linq.Dynamic.Core.Tests.Helpers.Models
{
    public enum SimpleValuesModelEnum
    {
        A,
        B
    }

    public class SimpleValuesModel
    {
        public int IntValue { get; set; }

        public float FloatValue { get; set; }

        public decimal DecimalValue { get; set; }

        public double DoubleValue { get; set; }

        public int? NullableIntValue { get; set; }

        public double? NullableDoubleValue { get; set; }

        public SimpleValuesModelEnum EnumValue { get; set; }
    }
}