namespace PhotoOrganizings.Interfaces;

public interface IPhotoOrganizerFactory
{
    PhotoOrganizer Create(PhotoOrganizerOptions options);
}