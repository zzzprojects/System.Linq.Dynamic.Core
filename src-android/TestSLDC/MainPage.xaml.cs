using System;
using Xamarin.Forms;

namespace TestSLDC
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
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
