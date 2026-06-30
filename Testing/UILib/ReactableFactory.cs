using Carbonate.NonDirectional;
using Carbonate.OneWay;

internal static class ReactableFactory
{
    private static IPushReactable<DisableMouseSubscriptionData>? disableMouseClickReactable;

    public static IPushReactable<DisableMouseSubscriptionData> CreateDisableMouseClickReactable()
    {
        if (disableMouseClickReactable is not null)
        {
            return disableMouseClickReactable;
        }
        
        disableMouseClickReactable = new PushReactable<DisableMouseSubscriptionData>();

        return disableMouseClickReactable;
    }
}