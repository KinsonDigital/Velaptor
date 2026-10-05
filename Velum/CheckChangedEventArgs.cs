namespace Velum;

public class CheckChangedEventArgs : EventArgs
{
    public CheckChangedEventArgs(bool isChecked) => IsChecked = isChecked;

    public bool IsChecked { get; }
}
