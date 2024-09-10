using Firebase.Storage;
using GundamStore.Interfaces;
using GundamStore.Models;
using Microsoft.Extensions.Options;



namespace GundamStore.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly FirebaseSettings _firebaseSettings;

        public FirebaseStorageService(IOptions<FirebaseSettings> firebaseSettings)
        {
            _firebaseSettings = firebaseSettings.Value;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty.", nameof(file));
            }

            var stream = file.OpenReadStream();

            try
            {
                var firebaseStorage = new FirebaseStorage(
                    _firebaseSettings.StorageBucket,
                    new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true
                    });

                var uploadTask = firebaseStorage
                    .Child(folder)
                    .Child(file.FileName)
                    .PutAsync(stream);

                var downloadUrl = await uploadTask;
                return downloadUrl;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while uploading the file.", ex);
            }
        }
    }
}
