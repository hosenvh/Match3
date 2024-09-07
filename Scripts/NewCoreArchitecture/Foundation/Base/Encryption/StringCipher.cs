
namespace PandasCanPlay.BaseGame.Foundation
{
    public interface StringCipher
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);

    }
}