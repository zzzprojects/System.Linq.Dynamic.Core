using System;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using ExpressionSample;
using Xamarin.Forms;

namespace TestSLDC
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var lambdaParameter = new[] { Expression.Parameter(typeof(MyEntity), string.Empty) };
            var workingExpression = DynamicExpressionParser.ParseLambda(lambdaParameter, typeof(bool), "MyEnum = MyEnum.SecondValue", null);

            try
            {
                var problematicExpression = DynamicExpressionParser.ParseLambda(lambdaParameter, typeof(bool), "MyEnum = 1", null);
            }
            catch (Exception exception)
            {
                // This should not happen
                int i = 42;
                MessageLbl.Text = exception.ToString();
            }
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                const string filterStr = "Brightness < 0.5f";
                var filter = filterStr.ParseLambda<IColorComponent, bool>();
                MessageLbl.Text = "Success";
            }
            catch (Exception exception)
            {
                MessageLbl.Text = exception.ToString();
            }
        }
    }
}
