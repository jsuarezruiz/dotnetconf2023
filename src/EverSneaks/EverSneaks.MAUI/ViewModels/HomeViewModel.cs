﻿using EverSneaks.MAUI.Views;
using System.Windows.Input;

namespace EverSneaks.MAUI.ViewModels
{
    internal class HomeViewModel
    {
        public ICommand GoToGalleryCommand { get; }

        public HomeViewModel()
        {
            this.GoToGalleryCommand = new Command(
                                        execute: async () => await Shell.Current.GoToAsync("gallery"),
                                        canExecute: () => Shell.Current.CurrentPage is HomeView);
        }
    }
}