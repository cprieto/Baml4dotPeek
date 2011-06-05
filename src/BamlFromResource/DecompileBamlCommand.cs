using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Reflector.BamlViewer;

namespace Cprieto.DotPeek
{
    public class DecompileBamlCommand : ICommand
    {
        public void Execute(object parameter)
        {
            var items = parameter as IList;
            if (items ==  null || items.Count == 0) return;

            var resource = items[0] as Resource;
            var window = new Window
                             {
                                 Content = new XamlViewer{ DataContext = new XamlViewerViewModel(resource)},
                                 Title = string.Format("Xaml source code for {0}", resource.Name),
                                 ShowInTaskbar = false,
                                 ResizeMode = ResizeMode.NoResize
                             };
            window.ShowDialog();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}