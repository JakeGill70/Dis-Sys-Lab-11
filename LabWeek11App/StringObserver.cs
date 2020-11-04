using System;
using System.Windows.Forms;

namespace LabWeek11App
{
    public class StringObserver : IObserver<string>
    {
        private IDisposable _unsubscriber;
        private readonly RichTextBox _txtBox;
        private readonly object _lockObject = new object();

        public StringObserver(RichTextBox txtBox)
        {
            _txtBox = txtBox;
        }

        public virtual void Subscribe(IObservable<string> provider)
        {
            _unsubscriber = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }

        public void OnCompleted()
        {
            lock (_lockObject)
            {
                _txtBox.BeginInvoke(new Action(
                   () => _txtBox.Text += "Completed." + Environment.NewLine
                ));
            }
        }

        public void OnError(Exception error)
        {
            lock (_lockObject)
            {
                _txtBox.BeginInvoke(new Action(
                   () => _txtBox.Text += "ERROR: " + error + Environment.NewLine
                ));
            }
        }

        public void OnNext(string value)
        {
            lock (_lockObject)
            {
                _txtBox.BeginInvoke(new Action(
                   () => _txtBox.Text += value + Environment.NewLine
                ));
            }
        }
    }
}
