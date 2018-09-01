using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOALearningEnglish.ViewModels
{
    class PlayVideoPageViewModel: ViewModelBase
    {
        public RelayCommand NavigateCommand { get; private set; }
        private readonly INavigationService _navigationService;


        public PlayVideoPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NavigateCommand = new RelayCommand(NavigateCommandAction);

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
                        BackButton();
                    });
                }
                return _clickBackButton;
            }
        }

        public void BackButton()
        {
            //_navigationService.NavigateTo("StartPage");
            _navigationService.GoBack();

        }

        private void NavigateCommandAction()
        {
            // do somethings in here

        }


    }
}
