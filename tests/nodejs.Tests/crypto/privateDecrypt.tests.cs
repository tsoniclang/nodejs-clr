using Xunit;
using System;

namespace nodejs.Tests;

public class privateDecryptTests
{
    [Fact]
    public void privateDecrypt_InvalidKeyType_Throws()
    {
        var (publicKey, _) = crypto.generateKeyPairSync("rsa", null);
        var ciphertext = new byte[256];

        Assert.Throws<ArgumentException>(() => crypto.privateDecrypt(publicKey, ciphertext));
    }
}
