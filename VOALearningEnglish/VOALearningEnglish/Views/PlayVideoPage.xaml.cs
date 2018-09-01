using MyToolkit.Multimedia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using VOALearningEnglish.Data;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Popups;
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
    public sealed partial class PlayVideoPage : Page
    {
        public PlayVideoPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    player.Visibility = Visibility.Collapsed;
                    progress.Visibility = Visibility.Visible;

                    string videoId = String.Empty;
                    YoutubeVideo video = e.Parameter as YoutubeVideo;
                    if (video != null && !video.Id.Equals(String.Empty))
                    {
                        if (video.IsLive)
                        {
                            var streamResponse = await AdaptiveMediaSource.CreateFromUriAsync(new Uri(video.YoutubeLink));
                            if (streamResponse.Status == AdaptiveMediaSourceCreationStatus.Success)
                            {
                                player.SetMediaStreamSource(streamResponse.MediaSource);
                            }
                        }
                        else
                        {
                            //-----Get The Video Uri and set it as a player source-------//
                            var url = await YouTube.GetVideoUriAsync(video.Id, YouTubeQuality.Quality480P);
                            player.Source = url.Uri;
                        }
                    }

                    player.Visibility = Visibility.Visible;
                    progress.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageDialog message = new MessageDialog("You're not connected to Internet!");
                    await message.ShowAsync();
                    this.Frame.GoBack();
                }
            }
            catch
            {

            }

            base.OnNavigatedTo(e);
        }

    }
}
