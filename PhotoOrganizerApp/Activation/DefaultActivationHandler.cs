using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace PhotoOrganizings.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    public DefaultActivationHandler()
    {
    }

    protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        await Task.CompletedTask;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        return true;
    }
}