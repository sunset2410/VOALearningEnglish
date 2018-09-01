using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VOALearningEnglish.Data;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VOALearningEnglish.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        DispatcherTimer dispatcherTimer;
        string textSelected;
        public StartPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            textSelected = string.Empty;
            DispatcherTimerSetup();
        }

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
            dispatcherTimer.Start();

        }
        async void dispatcherTimer_Tick(object sender, object e)
        {
            try
            {
                string[] arguments = new string[] { @"window.getSelection().toString();" };
                string s = await mainView.InvokeScriptAsync("eval", arguments);
                s = s.Trim();
                if ((s.CompareTo(string.Empty) != 0) && (s.CompareTo(textSelected) != 0))
                {
                    if (!s.Contains(" "))
                    {
                        textSelected = s;
                        s = s.ToLower();
                        string url = "http://www.oxfordlearnersdictionaries.com/definition/english/" + s.ToString() + "?q=" + s.ToString();
                        subView.Navigate(new Uri(url));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void ListVideo_ItemClick(object sender, ItemClickEventArgs e)
        {
            YoutubeVideo video = e.ClickedItem as YoutubeVideo;
            if (video != null)
                Frame.Navigate(typeof(PlayVideoPage), video);
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            (this.DataContext as ViewModels.StartPageViewModel).OnNavigatedTo(e);
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            (this.DataContext as ViewModels.StartPageViewModel).OnNavigatedFrom(e);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            (this.DataContext as ViewModels.StartPageViewModel).PageSizeChanged(sender, e);
        }
    }
}
