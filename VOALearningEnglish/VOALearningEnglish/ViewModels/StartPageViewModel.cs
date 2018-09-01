using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VOALearningEnglish.Data;
using VOALearningEnglish.Datas;
using VOALearningEnglish.Models;
using VOALearningEnglish.Settings;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace VOALearningEnglish.ViewModels
{
    class StartPageViewModel: ViewModelBase
    {

        #region field
        public ObservableCollection<ItemList> DataListNewChanels { get; set; }  // list news channel
        public ObservableCollection<YoutubeVideo> VideoList { get; set; }  // off line channel video list
        private AppSetting _appSettings = new AppSetting();
        private YoutubeManager youtubeManager = new YoutubeManager();
        CancellationTokenSource CancellationToken;

        // mode news or mode videos
        private bool _isModeNews = true;
        public bool IsModeNews
        {
            get
            {
                return _isModeNews;
            }
            set
            {
                _isModeNews = value;
                RaisePropertyChanged("IsModeNews");

            }
        }


        private Visibility _isShowSmallScreenMode = Visibility.Collapsed;
        public Visibility IsShowSmallScreenMode
        {
            get { return _isShowSmallScreenMode; }
            set
            {
                _isShowSmallScreenMode = value;
                RaisePropertyChanged("IsShowSmallScreenMode");
            }
        }


        // pan of splitview open or not?
        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get
            {
                return _isPaneOpen;
            }
            set
            {
                _isPaneOpen = value;
                RaisePropertyChanged("IsPaneOpen");

            }
        }

        // listNews open or not?
        private bool _isListNewsOpen = true;
        public bool IsListNewsOpen
        {
            get
            {
                return _isListNewsOpen;
            }
            set
            {
                _isListNewsOpen = value;
                RaisePropertyChanged("IsPaneOpen");
            }
        }

        // open large listnews or not?
        private Visibility _isShowLargeListNews;
        public Visibility IsShowLargeListNews
        {
            get { return _isShowLargeListNews; }
            set
            {
                _isShowLargeListNews = value;
                RaisePropertyChanged("IsShowLargeListNews");
            }
        }

        public const string TURN_ON_DICTIONARY = "Turn On Dictionary";
        public const string TURN_OFF_DICTIONARY = "Turn Off Dictionary";

        public const string SHOW = "Show";
        public const string HIDE = "Hide";

       
        public const string OPEN_ANIMATION = "Open";
        public const string CLOSE_ANIMATION = "Close";

        internal void OnNavigatedFrom(NavigationEventArgs e)
        {
            SaveInfo();
        }

        //--- ListNewsAnimation------
        private string _listNewsAnimation = string.Empty;
        public string ListNewsAnimation
        {
            get { return _listNewsAnimation; }
            set
            {
                _listNewsAnimation = value;
                RaisePropertyChanged("ListNewsAnimation");
            }
        }


        // show Mode News or not?
        private Visibility _isShowModeNews;
        public Visibility IsShowModeNews
        {
            get { return _isShowModeNews; }
            set
            {
                _isShowModeNews = value;
                RaisePropertyChanged("IsShowModeNews");
            }
        }


        // show Mode Videos or not?
        private Visibility _isShowModeVideos;
        public Visibility IsShowModeVideos
        {
            get { return _isShowModeVideos; }
            set
            {
                _isShowModeVideos = value;
                RaisePropertyChanged("IsShowModeVideos");
            }
        }

        // string server path of mainview
        private string _serverPre = string.Empty;
        private string _serverPath = string.Empty;
        public string ServerPath
        {
            get { return _serverPath; }
            set
            {
                _serverPath = value;
                RaisePropertyChanged("ServerPath");
            }
        }

        private string _dictURL = "http://www.oxfordlearnersdictionaries.com/";
        public string DictURL
        {
            get { return _dictURL; }
            set
            {
                _dictURL = value;
                RaisePropertyChanged("DictURL");
            }
        }

        private string _textToolTipDictionary = string.Empty;
        public string TextToolTipDictionary
        {
            get { return _textToolTipDictionary; }
            set
            {
                _textToolTipDictionary = value;
                RaisePropertyChanged("TextToolTipDictionary");
            }
        }

        private string _textToolTipListNews = string.Empty;
        public string TextToolTipListNews
        {
            get { return _textToolTipListNews; }
            set
            {
                _textToolTipListNews = value;
                RaisePropertyChanged("TextToolTipListNews");
            }
        }

        // show dictionnary or not?
        private Visibility _isShowSubView;
        public Visibility IsShowSubView
        {
            get { return _isShowSubView; }
            set
            {
                _isShowSubView = value;
                RaisePropertyChanged("IsShowSubView");
            }
        }


        // load webview or not?
        private bool _isActiveProgressRing = false;
        public bool IsActiveProgressRing
        {
            get
            {
                return _isActiveProgressRing;
            }
            set
            {
                _isActiveProgressRing = value;
                RaisePropertyChanged("IsActiveProgressRing");

            }
        }


        // load list video on a chanel or not?
        private bool _isActiveProgressRingLoadVideo = false;
        public bool IsActiveProgressRingLoadVideo
        {
            get
            {
                return _isActiveProgressRingLoadVideo;
            }
            set
            {
                _isActiveProgressRingLoadVideo = value;
                RaisePropertyChanged("IsActiveProgressRingLoadVideo");

            }
        }


        // bool on off Dictionnary togg button
        private bool _isDictionnaryOn = false;
        public bool IsDictionnaryOn
        {
            get { return _isDictionnaryOn; }
            set
            {
                _isDictionnaryOn = value;
                RaisePropertyChanged("IsDictionnaryOn");
            }
        }


        private bool IsPcDevice = true;

        #endregion


        #region method_command
        public StartPageViewModel()
        {

            try
            {
                IsPaneOpen = false;
                ContructListView();
            }
            catch (Exception ex)
            {

            }

            //Windows.Desktop , Windows.Mobile
            var deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            if (deviceFamily.CompareTo("Windows.Desktop") == 0)
            {
                IsPcDevice = true;
            }
            else
            {
                IsPcDevice = false;
            }


        }

        public void ContructListView()
        {
            ServerPath = "http://learningenglish.voanews.com/";
            _serverPre = ServerPath;
            IsModeNews = true;
            if (IsModeNews)
            {
                IsShowModeNews = Visibility.Visible;
                IsShowModeVideos = Visibility.Collapsed;
            }
            else
            {
                IsShowModeNews = Visibility.Collapsed;
                IsShowModeVideos = Visibility.Visible;
            }

            // ----------read infomation from app setting------ //
            if (_appSettings.IsContainKey(SettingKey.IsDictionnaryOn.ToString()))
                IsDictionnaryOn = _appSettings.ReadSettingBool(SettingKey.IsDictionnaryOn.ToString());
            if (_appSettings.IsContainKey(SettingKey.IsListNewsOpen.ToString()))
                IsListNewsOpen = _appSettings.ReadSettingBool(SettingKey.IsListNewsOpen.ToString());

            //
            if (IsDictionnaryOn)
            {
                IsShowSubView = Visibility.Visible;
                TextToolTipDictionary = TURN_OFF_DICTIONARY;
            }
            else
            {
                IsShowSubView = Visibility.Collapsed;
                TextToolTipDictionary = TURN_ON_DICTIONARY;
            }

            //
            if (IsListNewsOpen)
            {
                ListNewsAnimation = OPEN_ANIMATION;
                IsShowLargeListNews = Visibility.Visible;
                TextToolTipListNews = HIDE;
            }
            else
            {
                ListNewsAnimation = CLOSE_ANIMATION;
                IsShowLargeListNews = Visibility.Collapsed;
                TextToolTipListNews = SHOW;
            }

            getAllContentVOA();
        }


        internal void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                // ----------read infomation from app setting------ //
                if (_appSettings.IsContainKey(SettingKey.IsDictionnaryOn.ToString()))
                    IsDictionnaryOn = _appSettings.ReadSettingBool(SettingKey.IsDictionnaryOn.ToString());
                if (_appSettings.IsContainKey(SettingKey.IsListNewsOpen.ToString()))
                    IsListNewsOpen = _appSettings.ReadSettingBool(SettingKey.IsListNewsOpen.ToString());

                //
                if (IsDictionnaryOn)
                {
                    IsShowSubView = Visibility.Visible;
                    TextToolTipDictionary = TURN_OFF_DICTIONARY;
                }
                else
                {
                    IsShowSubView = Visibility.Collapsed;
                    TextToolTipDictionary = TURN_ON_DICTIONARY;
                }

                //
                if (IsListNewsOpen)
                {
                    ListNewsAnimation = OPEN_ANIMATION;
                    IsShowLargeListNews = Visibility.Visible;
                    TextToolTipListNews = HIDE;
                }
                else
                {
                    ListNewsAnimation = CLOSE_ANIMATION;
                    IsShowLargeListNews = Visibility.Collapsed;
                    TextToolTipListNews = SHOW;
                }
            }
            catch (Exception ex)
            {

            }
            

        }


        public void getAllContentVOA()
        {
            try
            {
                DataListNewChanels = new ObservableCollection<ItemList>();

                // main page
                ItemList c1 = new ItemList() { Type = ItemType.New, TextItem = "Voice of America", TextToolTip = "Voice of America", ImageSource = "ms-appx:///../Images/voa.png", UrlLink = "http://learningenglish.voanews.com/" };
                // newspaper
                ItemList c2 = new ItemList() { Type = ItemType.New, TextItem = "As It Is", TextToolTip = "As It Is", ImageSource = "ms-appx:///../Images/as_it_is.png", UrlLink = "http://learningenglish.voanews.com/z/3521" };
                ItemList c3 = new ItemList() { Type = ItemType.New, TextItem = "Health & Lifestyle", TextToolTip = "Health & Lifestyle", ImageSource = "ms-appx:///../Images/Health_Lifestyle.jpg", UrlLink = "http://learningenglish.voanews.com/z/955" };
                ItemList c4 = new ItemList() { Type = ItemType.New, TextItem = "Science in the News", TextToolTip = "Science in the News", ImageSource = "ms-appx:///../Images/Science_in_the_News.jpg", UrlLink = "http://learningenglish.voanews.com/z/1579" };
                ItemList c5 = new ItemList() { Type = ItemType.New, TextItem = "Words and Their Stories", TextToolTip = "Words and Their Stories", ImageSource = "ms-appx:///../Images/Words_and_Their_Stories.jpg", UrlLink = "http://learningenglish.voanews.com/z/987" };

                // video page
                ItemList c7 = new ItemList() { Type = ItemType.New, TextItem = "English In A Minute", TextToolTip = "English In A Minute", ImageSource = "ms-appx:///../Images/English_In_A_Minute.jpg", UrlLink = "http://learningenglish.voanews.com/z/3619" };
                ItemList c8 = new ItemList() { Type = ItemType.New, TextItem = "Everyday Grammar TV", TextToolTip = "Everyday Grammar TV", ImageSource = "ms-appx:///../Images/Everyday_Grammar_TV.jpg", UrlLink = "http://learningenglish.voanews.com/z/4716" };
                ItemList c9 = new ItemList() { Type = ItemType.New, TextItem = "Learning English Broadcast", TextToolTip = "Learning English Broadcast", ImageSource = "ms-appx:///../Images/Learning_English_Broadcast.jpg", UrlLink = "http://learningenglish.voanews.com/z/1689" };
                ItemList v10 = new ItemList() { Type = ItemType.Video, TextItem = "VOA News Tube", TextToolTip = "VOA News Tube", ImageSource = "ms-appx:///../Images/voanewstube.png", UrlLink = "VOAvideo" };

                DataListNewChanels.Add(c1);
                DataListNewChanels.Add(v10);
                DataListNewChanels.Add(c2);
                DataListNewChanels.Add(c3);
                DataListNewChanels.Add(c4);
                DataListNewChanels.Add(c5);
                DataListNewChanels.Add(c7);
                DataListNewChanels.Add(c8);
                DataListNewChanels.Add(c9);
                VideoList = new ObservableCollection<YoutubeVideo>();
            }
            catch(Exception ex)
            {

            }
           
        }

        private async void Parsing(string website)
        {
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(website);
                String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                source = WebUtility.HtmlDecode(source);
                HtmlDocument resultat = new HtmlDocument();
                resultat.LoadHtml(source);
                HtmlNode element = resultat.GetElementbyId("rssItems");
            }
            catch (Exception)
            {

            }
        }

        // click on ChangePanel off to on and reverse
        private RelayCommand _clickBackButton = null;
        public RelayCommand ClickBackButton
        {
            get
            {
                if (_clickBackButton == null)
                {
                    _clickBackButton = new RelayCommand(() =>
                    {
                        BackButtonRequest();
                    });
                }
                return _clickBackButton;
            }
        }

        private void BackButtonRequest()
        {
            if(_serverPre.CompareTo(String.Empty)!=0)
            {
                ServerPath = _serverPre;
            }

        }



        // click on ChangePanel off to on and reverse
        private RelayCommand _clickChangePanel = null;
        public RelayCommand ClickChangePanel
        {
            get
            {
                if (_clickChangePanel == null)
                {
                    _clickChangePanel = new RelayCommand(() =>
                    {
                        ChangePanel();
                    });
                }
                return _clickChangePanel;
            }
        }


        private void ChangePanel()
        {
            IsPaneOpen = !IsPaneOpen;
        }


        // click moreListNewsButon
        private RelayCommand _clickMoreListNews = null;
        public RelayCommand ClickMoreListNews
        {
            get
            {
                if (_clickMoreListNews == null)
                {
                    _clickMoreListNews = new RelayCommand(() =>
                    {
                        MoreListNews();
                        SaveInfo();
                    });
                }
                return _clickMoreListNews;
            }
        }

        private void MoreListNews()
        {
            IsListNewsOpen = !IsListNewsOpen;
            if (IsListNewsOpen)
            {
                IsShowLargeListNews = Visibility.Visible;
                ListNewsAnimation = OPEN_ANIMATION;
                TextToolTipListNews = HIDE;
            }
            else
            {
                IsShowLargeListNews = Visibility.Collapsed;
                ListNewsAnimation = CLOSE_ANIMATION;
                TextToolTipListNews = SHOW;
            }
        }

        // click OnOffDictionnary by ToggleSwitch
        private RelayCommand _clickOnOffDictionnaryToggleSwitch = null;
        public RelayCommand ClickOnOffDictionnaryToggleSwitch
        {
            get
            {
                if (_clickOnOffDictionnaryToggleSwitch == null)
                {
                    _clickOnOffDictionnaryToggleSwitch = new RelayCommand(() =>
                    {
                        OnOffDictionnaryToggleSwitch();
                        SaveInfo();
                    });
                }
                return _clickOnOffDictionnaryToggleSwitch;
            }
        }

        private void OnOffDictionnaryToggleSwitch()
        {
            IsShowSubView = (IsShowSubView == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            if (IsShowSubView == Visibility.Visible)
            {
                IsDictionnaryOn = true;
                TextToolTipDictionary = TURN_OFF_DICTIONARY;
            }
            else
            {
                IsDictionnaryOn = false;
                TextToolTipDictionary = TURN_ON_DICTIONARY;
            }
        }


        // webview content loading
        private RelayCommand<WebView> _onDisplay_ContentLoading;
        public RelayCommand<WebView> onDisplay_ContentLoading
        {
            get
            {
                return this._onDisplay_ContentLoading
                    ?? (this._onDisplay_ContentLoading = new RelayCommand<WebView>(item =>
                    {
                        IsActiveProgressRing = true;
                        WebViewContentLoading(item);
                    }));
            }
        }



        // webview content loaded
        private RelayCommand<WebView> _onDisplay_ContentLoaded;
        public RelayCommand<WebView> onDisplay_ContentLoaded
        {
            get
            {
                return this._onDisplay_ContentLoaded
                    ?? (this._onDisplay_ContentLoaded = new RelayCommand<WebView>(item =>
                    {
                        IsActiveProgressRing = false;
                        WebViewContentLoaded(item);

                    }));
            }
        }

        private async void WebViewContentLoaded(WebView item)
        {
            try
            {
                var display = (Windows.UI.Xaml.Controls.WebView)item;
                string html = await display.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });

                var appView = ApplicationView.GetForCurrentView();
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

                if (size.Width < 1000 || size.Height < 600 || !IsPcDevice)
                {
                    IsShowSmallScreenMode = Visibility.Collapsed;
                    IsShowSubView = Visibility.Collapsed;
                }
                else
                {
                    IsShowSmallScreenMode = Visibility.Visible;
                    if (IsDictionnaryOn)
                    {
                        IsShowSubView = Visibility.Visible;
                        TextToolTipDictionary = TURN_OFF_DICTIONARY;
                    }
                    else
                    {
                        IsShowSubView = Visibility.Collapsed;
                        TextToolTipDictionary = TURN_ON_DICTIONARY;
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public async void WebViewContentLoading(object item)
        {
            var display = (Windows.UI.Xaml.Controls.WebView)item;
            string html = await display.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });

        }


        // Dict webview content loading
        private RelayCommand<WebView> _subView_ContentLoading;
        public RelayCommand<WebView> SubView_ContentLoading
        {
            get
            {
                return this._subView_ContentLoading
                    ?? (this._subView_ContentLoading = new RelayCommand<WebView>(item =>
                    {
                        SubViewContentLoading(item);
                    }));
            }
        }

        private async void SubViewContentLoading(WebView item)
        {
            var display = (Windows.UI.Xaml.Controls.WebView)item;
            string html = await display.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
            string url = item.Source.AbsoluteUri.ToString();
            if (!url.Contains("oxfordlearnersdictionaries.com")) DictURL = "http://www.oxfordlearnersdictionaries.com/";
        }



        // webview content loaded
        private RelayCommand<WebView> _subView_ContentLoaded;
        public RelayCommand<WebView> SubView_ContentLoaded
        {
            get
            {
                return this._subView_ContentLoaded
                    ?? (this._subView_ContentLoaded = new RelayCommand<WebView>(item =>
                    {
                        SubViewContentLoaded(item);

                    }));
            }
        }

        private async void SubViewContentLoaded(WebView item)
        {
            var display = (Windows.UI.Xaml.Controls.WebView)item;
            string html = await display.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
        }



        //----- command selected item on list news chanel----------//
        private RelayCommand<ItemList> _selectedItemNewChanelCommand;
        public RelayCommand<ItemList> SelectedItemNewChanelCommand
        {
            get
            {
                return this._selectedItemNewChanelCommand
                    ?? (this._selectedItemNewChanelCommand = new RelayCommand<ItemList>(async item =>
                    {
                        //---------- check network ----------//
                        if (!NetworkInterface.GetIsNetworkAvailable())
                        {
                            MessageDialog message = new MessageDialog("You're not connected to Internet!");
                            await message.ShowAsync();
                        }

                        if (item.Type == ItemType.New && item.UrlLink.CompareTo(string.Empty) != 0)
                        {
                            await Task.Run(async () =>
                            {
                                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                () =>
                                {
                                    ServerPath = item.UrlLink;
                                    _serverPre = ServerPath;
                                    IsActiveProgressRing = true;
                                    IsModeNews = true;
                                    if (IsModeNews)
                                    {
                                        IsShowModeNews = Visibility.Visible;
                                        IsShowModeVideos = Visibility.Collapsed;
                                    }
                                    else
                                    {
                                        IsShowModeNews = Visibility.Collapsed;
                                        IsShowModeVideos = Visibility.Visible;
                                    }

                                }
                                );

                            });
                        }
                        else if(item.Type == ItemType.Video && item.UrlLink.CompareTo(string.Empty) != 0)
                        {
                            // reset server news to fix issue continue play video on news page when change video page
                            ServerPath = _serverPre;

                            if (CancellationToken != null)
                            {
                                CancellationToken.Cancel();
                                CancellationToken.Dispose();
                                CancellationToken = null;
                                CancellationToken = new CancellationTokenSource();
                            }
                            else
                            {
                                CancellationToken = new CancellationTokenSource();
                            }

                            //----- change mode to video mode-------//
                            IsModeNews = false;
                            if (IsModeNews)
                            {
                                IsShowModeNews = Visibility.Visible;
                                IsShowModeVideos = Visibility.Collapsed;
                            }
                            else
                            {
                                IsShowModeNews = Visibility.Collapsed;
                                IsShowModeVideos = Visibility.Visible;
                            }

                            //------ get list video and show to grid view-------//
                            try
                            {
                                IsActiveProgressRingLoadVideo = true;

                                await Task.Run(async () =>
                                {

                                    try
                                    {
                                        await GetListVideoOnChanel(item.UrlLink, CancellationToken.Token);
                                    }
                                    catch (OperationCanceledException ex)
                                    {
                                        if (CancellationToken != null)
                                        {
                                            CancellationToken.Cancel();
                                            CancellationToken.Dispose();
                                            CancellationToken = null;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (CancellationToken != null)
                                        {
                                            CancellationToken.Cancel();
                                            CancellationToken.Dispose();
                                            CancellationToken = null;
                                        }
                                    }

                                }, CancellationToken.Token);

                                if (CancellationToken != null)
                                {
                                    CancellationToken.Cancel();
                                    CancellationToken.Dispose();
                                    CancellationToken = null;
                                }

                                IsActiveProgressRingLoadVideo = false;
                            }
                            catch (Exception ex)
                            {
                                if (CancellationToken != null)
                                {
                                    CancellationToken.Cancel();
                                    CancellationToken.Dispose();
                                    CancellationToken = null;
                                }
                                IsActiveProgressRingLoadVideo = false;
                            }

                        }

                    }));
            }
        }

        public async Task<bool> GetListVideoOnChanel(string ChanelName, CancellationToken token)
        {
            if (token.IsCancellationRequested) throw new OperationCanceledException();

            List<YoutubeVideo> listVideo;
            try
            {
                //----- get all list video on channel-----------//
                listVideo = await youtubeManager.GetListVideos(ChanelName, token);

                if (token.IsCancellationRequested) throw new OperationCanceledException();

                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    try
                    {
                        //-------remove video on list-----------//
                        for (int i = VideoList.Count - 1; i >= 0; i--)
                        {
                            VideoList.RemoveAt(i);
                        }

                        //--- add list video-----//
                        for (int i = 0; i < listVideo.Count; i++)
                        {
                            VideoList.Add(listVideo[i]);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new OperationCanceledException();
                    }

                }

                );

            }
            catch (Exception ex)
            {
                throw new OperationCanceledException();
            }
            return true;
        }


        public void SaveInfo()
        {
            // ----- write infomation from app setting------ //
            _appSettings.WriteSettingBool(SettingKey.IsDictionnaryOn.ToString(), IsDictionnaryOn);
            _appSettings.WriteSettingBool(SettingKey.IsListNewsOpen.ToString(), IsListNewsOpen);
        }


        public void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window.Current.CoreWindow.SizeChanged += (ss, ee) =>
            {
                try
                {
                    var appView = ApplicationView.GetForCurrentView();
                    var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                    var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                    var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

                    if (size.Width < 1000 || size.Height < 600 || !IsPcDevice)
                    {
                        IsShowSmallScreenMode = Visibility.Collapsed;
                        IsShowSubView = Visibility.Collapsed;
                    }
                    else
                    {
                        IsShowSmallScreenMode = Visibility.Visible;
                        if (IsDictionnaryOn)
                        {
                            IsShowSubView = Visibility.Visible;
                            TextToolTipDictionary = TURN_OFF_DICTIONARY;
                        }
                        else
                        {
                            IsShowSubView = Visibility.Collapsed;
                            TextToolTipDictionary = TURN_ON_DICTIONARY;
                        }
                    }
                    ee.Handled = true;
                }
                catch( Exception ex)
                {

                }
            };

        }

        #endregion

    }
}
