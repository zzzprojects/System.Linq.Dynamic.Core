using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ExpressionSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            TryParseExpression();
        }

        private void TryParseExpression()
        {
            var lambdaParameter = new[] { Expression.Parameter(typeof(MyEntity), string.Empty) };
            var workingExpression = DynamicExpressionParser.ParseLambda(lambdaParameter, typeof(bool), "MyEnum = MyEnum.SecondValue", null);

            try
            {
                var problematicExpression = DynamicExpressionParser.ParseLambda(lambdaParameter, typeof(bool), "MyEnum = 1", null);
                int ok = 0;
            }
            catch (Exception e)
            {
                // This should not happen
                int y = 9;
                throw;
            }
        }
    }
}
