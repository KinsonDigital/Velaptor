namespace Velum;

using Carbonate.OneWay;

internal static class ReactableFactory
{
    private static bool isDisposed;

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

    public static void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        disableMouseClickReactable?.Dispose();
        disableMouseClickReactable = null;

        isDisposed = true;
    }
}
