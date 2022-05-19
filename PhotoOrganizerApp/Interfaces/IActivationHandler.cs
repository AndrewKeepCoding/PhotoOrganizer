using System.Threading.Tasks;

namespace PhotoOrganizings.Interfaces;

public interface IActivationHandler
{
    bool CanHandle(object args);

    Task HandleAsync(object args);
}