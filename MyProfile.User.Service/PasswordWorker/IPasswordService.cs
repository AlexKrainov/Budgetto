namespace MyProfile.User.Service.PasswordWorker
{
    public interface IPasswordService
	{
		string GeneratePassword(int length);
		string GenerateSalt(int length = 32);
		byte[] GenerateHashSHA256(string password, byte[] salt, int iterations = 10000, int hashByteSize = 32);
		string GenerateHashSHA256(string password, string saltAsBase64String, int iterations = 10000, int hashByteSize = 32);
	}
}
