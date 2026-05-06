using System;
using FundForest.Helpers;

namespace FundForest.Services
{
    /// <summary>
    /// Simple navigation service using event-based page switching.
    /// The MainViewModel subscribes and swaps the CurrentView.
    /// </summary>
    public class NavigationService
    {
        public event Action<string>? NavigationRequested;

        public void NavigateTo(string pageName) => NavigationRequested?.Invoke(pageName);
    }
}