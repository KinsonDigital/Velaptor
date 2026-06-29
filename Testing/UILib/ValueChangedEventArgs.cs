public class ValueChangedEventArgs : EventArgs
{
    public ValueChangedEventArgs(float oldValue, float newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public float OldValue { get; }

    public float NewValue { get; }
}
