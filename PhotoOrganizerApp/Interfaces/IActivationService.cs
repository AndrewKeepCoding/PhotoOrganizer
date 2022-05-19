using System.Threading.Tasks;

namespace PhotoOrganizings.Interfaces;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}