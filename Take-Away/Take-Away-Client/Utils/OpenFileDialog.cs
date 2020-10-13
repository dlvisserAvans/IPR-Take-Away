using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Take_Away_Client.Utils
{
    public class OpenFilesDialog : IFileDialogWindow
    {
        public List<string> ExecuteFileDialog(object owner, string extFilter)
        {
            var fd = new OpenFileDialog();
            fd.Multiselect = true;
            if (!string.IsNullOrWhiteSpace(extFilter))
            {
                fd.Filter = extFilter;
            }
            fd.ShowDialog(owner as Window);

            return fd.FileNames.ToList();
        }
    }
}
