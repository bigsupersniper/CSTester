using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CSTester.CSEngine
{
    public class TextWriterProvider : TextWriter
    {
        public TextBox Owner { get; private set; }

        public TextWriterProvider(TextBox textBox)
        {
            Owner = textBox;
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Write(string value)
        {
            if (value == null) return;

            Owner.Dispatcher.Invoke(() =>
            {
                Owner.AppendText(value);
                Owner.ScrollToEnd();
            });
        }

        public override void Write(object value)
        {
            if (value == null) return;

            Owner.Dispatcher.Invoke(() =>
            {
                Owner.AppendText(value.ToString());
                Owner.ScrollToEnd();
            });
        }

        public override async Task WriteAsync(string value)
        {
            if (value == null) return;

            await Owner.Dispatcher.InvokeAsync(() =>
            {
                Owner.AppendText(value);
                Owner.ScrollToEnd();
            });
        }

        public override void WriteLine()
        {
            Owner.Dispatcher.Invoke(() =>
            {
                Owner.AppendText(Environment.NewLine);
                Owner.ScrollToEnd();
            });
        }

        public override void WriteLine(string value)
        {
            if (value == null) return;

            Owner.Dispatcher.Invoke(() =>
            {
                Owner.AppendText(value.ToString() + Environment.NewLine);
                Owner.ScrollToEnd();
            });
        }

        public override void WriteLine(object value)
        {
            if (value == null) return;

            Owner.Dispatcher.Invoke(() =>
            {
                Owner.AppendText(value.ToString() + Environment.NewLine);
                Owner.ScrollToEnd();
            });
        }

        public override async Task WriteLineAsync(string value)
        {
            if (value == null) return;

            await Owner.Dispatcher.InvokeAsync(() =>
            {
                Owner.AppendText(value.ToString() + Environment.NewLine);
                Owner.ScrollToEnd();
            });
        }
    }
}
