using System;
using System.Collections.Generic;
using System.Text;

namespace Take_Away_Client.Utils
{
    using System.Collections.Generic;
    public interface IFileDialogWindow
    {
        List<string> ExecuteFileDialog(object owner, string extFilter);
    }
}
