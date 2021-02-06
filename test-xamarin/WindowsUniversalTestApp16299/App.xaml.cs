using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Linq.Dynamic.Core;

namespace WindowsUniversalTestApp16299
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            GlobalConfig.CustomTypeProvider = new WindowsAppCustomTypeProvider();

            var containsList = new List<int> { 0, 1, 2, 3 };
            var q = containsList.AsQueryable().Where("it > 1");
            var a = q.ToDynamicArray<int>();


            var lst = new List<TestEnum> { TestEnum.Var1, TestEnum.Var2, TestEnum.Var3, TestEnum.Var4, TestEnum.Var5, TestEnum.Var6 };
            var qry = lst.AsQueryable();

            //Act
            var result1 = qry.Where("it < TestEnum.Var4").ToDynamicList();
            var result2 = qry.Where("TestEnum.Var4 > it").ToDynamicList();
            var result3 = qry.Where("it = Var5").ToDynamicList();
            var result4 = qry.Where("it = @0", TestEnum.Var5).ToDynamicList();
            var result5 = qry.Where("it = @0", 8).ToDynamicList();

            var resultNewList1 = containsList.AsQueryable().Select("new (it as i)").ToDynamicList();
            var resultNew1 = resultNewList1[0];
            int i1 = resultNew1.i;

            var resultNew4List = containsList.AsQueryable().Select("new (it as a, it as b, \"c\" as c, 200 as d)").ToDynamicList();
            var resultNew4 = resultNew4List[0];
            int a4 = resultNew4.a;
            int b4 = resultNew4.b;
            string c4 = resultNew4.c;
            int d4 = resultNew4.d;

            var resultNewList5TestClass = containsList.AsQueryable().Select<TestClass>("new (it as a, it as b, \"c\" as c, 200 as d)").ToList();
            var firstTestClass = resultNewList5TestClass.First();

            var resultNewList4WithMyClass = containsList.AsQueryable().Select<TestClassWithConstructor>("new (it as a, it as b, \"c\" as c, 200 as d)").ToList();
            var firstTestClassWithConstructor = resultNewList4WithMyClass.First();

            var resultNewList5 = containsList.AsQueryable().Select("new (it as a, it as b, \"c\" as c, 200 as d, 300 as e)").ToDynamicList();
            int y = 0;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
