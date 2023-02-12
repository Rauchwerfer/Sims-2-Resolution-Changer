using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sims2ResolutionChanger
{
    internal class Logger : ILogger
    {
        private TextBox _logTextBox;
        
        public Logger(TextBox logTextBox) 
        {
            _logTextBox = logTextBox;
        }

        public void Log(string message)
        {
            _logTextBox.AppendText(message + Environment.NewLine);
        }
    }
}
