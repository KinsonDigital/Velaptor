public class SelectedItemChangedEventArgs : EventArgs
{
    public SelectedItemChangedEventArgs(string oldValue, string newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public string OldValue { get; }

    public string NewValue { get; }
}
