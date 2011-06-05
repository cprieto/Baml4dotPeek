using System;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace Cprieto.DotPeek
{
    [ActionHandler("Cprieto.ShowBamlResources")]
    public class BamlActionHandler : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            System.Diagnostics.Trace.WriteLine("*** Hello!!! ***");
        }
    }
}