namespace GundamStore.Interfaces
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
    }
}
