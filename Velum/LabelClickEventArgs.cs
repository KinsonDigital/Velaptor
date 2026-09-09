namespace Velum;

public class LabelClickEventArgs : EventArgs
{
    public LabelClickEventArgs(Label label) => Label = label;

    public Label Label { get; }
}
