using PhotoOrganizings.Interfaces;
using System.Threading.Tasks;

namespace PhotoOrganizings.Activation;

public abstract class ActivationHandler<T> : IActivationHandler where T : class
{
    public async Task HandleAsync(object args)
    {
        if (args is T castedArgs)
        {
            await HandleInternalAsync(castedArgs);
        }
    }

    public bool CanHandle(object args)
    {
        if (args is T castedArgs)
        {
            return args is T && CanHandleInternal(castedArgs);
        }

        return false;
    }

    protected abstract Task HandleInternalAsync(T args);

    protected virtual bool CanHandleInternal(T args)
    {
        return true;
    }
}