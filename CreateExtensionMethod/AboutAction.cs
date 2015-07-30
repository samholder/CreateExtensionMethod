using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace CreateExtensionMethod
{
    using JetBrains.UI.ActionsRevised;

    [ActionHandler(typeof(AboutAction))]
    public class AboutAction : IExecutableAction
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // return true or false to enable/disable this action
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            MessageBox.Show(
              "Generate Extension Method\nSam Holder\n\nAdds a quick fix which allows a new extension method to be generated",
              "About Generate Extension Method",
              MessageBoxButtons.OK,
              MessageBoxIcon.Information);
        }
    }
}