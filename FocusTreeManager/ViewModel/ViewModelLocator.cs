/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:FocusTreeManager"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System.Windows;

namespace FocusTreeManager.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ProjectViewViewModel>();
            SimpleIoc.Default.Register<AddFocusViewModel>();
            SimpleIoc.Default.Register<EditFocusViewModel>();
            SimpleIoc.Default.Register<ChangeImageViewModel>();
            Messenger.Default.Register<NotificationMessage>(this, NotifyUserMethod);
        }

        private void NotifyUserMethod(NotificationMessage obj)
        {
            //Notify
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public AddFocusViewModel AddFocus_Flyout
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddFocusViewModel>();
            }
        }

        public EditFocusViewModel EditFocus_Flyout
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditFocusViewModel>();
            }
        }

        public ChangeImageViewModel ChangeImage
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChangeImageViewModel>();
            }
        }

        public ProjectViewViewModel ProjectView
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProjectViewViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}