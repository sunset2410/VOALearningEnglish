using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOALearningEnglish.Views;

namespace VOALearningEnglish.ViewModels
{
    class ViewModelLocator
    {

        public const string StartPageKey = "StartPage";
        public const string PlayVideoPageKey = "PlayVideoPage";

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var nav = new NavigationService();
            nav.Configure(StartPageKey, typeof(StartPage));
            nav.Configure(PlayVideoPageKey, typeof(PlayVideoPage));



            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
            }

            //Register your services used here
            SimpleIoc.Default.Register<INavigationService>(() => nav);
            SimpleIoc.Default.Register<StartPageViewModel>();
            SimpleIoc.Default.Register<PlayVideoPageViewModel>();

        }


        // <summary>
        // Gets the StartPage view model.
        // </summary>
        // <value>
        // The StartPage view model.
        // </value>
        public StartPageViewModel StartPageInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StartPageViewModel>();
            }
        }



        // <summary>
        // Gets the PlayVideoPage view model.
        // </summary>
        // <value>
        // The PlayVideoPage view model.
        // </value>
        public PlayVideoPageViewModel PlayVideoPageInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlayVideoPageViewModel>();
            }
        }


        // <summary>
        // The cleanup.
        // </summary>
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }


    }
}
