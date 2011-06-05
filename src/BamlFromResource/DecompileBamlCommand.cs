using System;
using System.Collections;
using System.Collections.Generic;
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
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}