internal struct DisableMouseSubscriptionData
{
    /// <summary>
    /// Gets or sets a value indicating whether any dropdown is currently expanded.
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Gets or sets the ID of the dropdown that is currently expanded.
    /// 0 means no dropdown is expanded.
    /// </summary>
    public int ExpandedDropDownId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a mouse click was consumed
    /// by a dropdown item selection during the current frame. When set,
    /// all other dropdowns should ignore click processing for this frame.
    /// </summary>
    public bool ConsumedClick { get; set; }
}
