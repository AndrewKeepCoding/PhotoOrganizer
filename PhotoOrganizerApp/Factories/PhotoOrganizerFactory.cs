using PhotoOrganizings.Interfaces;

namespace PhotoOrganizings.Factories;

public class PhotoOrganizerFactory : IPhotoOrganizerFactory
{
    public PhotoOrganizer Create(PhotoOrganizerOptions options) => new PhotoOrganizer(options);
}