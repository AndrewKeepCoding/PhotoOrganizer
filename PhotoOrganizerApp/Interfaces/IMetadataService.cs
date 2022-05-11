using System;
using System.Threading.Tasks;

namespace PhotoOrganizings.Interfaces;

public interface IMetadataService
{
    Task<string?> GetHumanizedFileSize(string filePath);

    DateTime? GetTakenDate(string filePath);
}