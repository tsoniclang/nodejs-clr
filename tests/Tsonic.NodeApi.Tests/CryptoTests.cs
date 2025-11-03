using Xunit;
using System;
using System.Text;

namespace Tsonic.NodeApi.Tests;

public class CryptoTests
{
    [Fact]
    public void Hash_SHA256_ProducesCorrectDigest()
    {
        var hash = crypto.createHash("sha256");
        hash.update("Hello, World!");
        var digest = hash.digest("hex");

        Assert.Equal("dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f", digest);
    }

    [Fact]
    public void Hash_MD5_ProducesCorrectDigest()
    {
        var hash = crypto.createHash("md5");
        hash.update("Hello, World!");
        var digest = hash.digest("hex");

        Assert.Equal("65a8e27d8879283831b664bd8b7f0ad4", digest);
    }

    [Fact]
    public void Hash_MultipleUpdates_ProducesCorrectDigest()
    {
        var hash = crypto.createHash("sha256");
        hash.update("Hello");
        hash.update(", ");
        hash.update("World!");
        var digest = hash.digest("hex");

        Assert.Equal("dffd6021bb2bd5b0af676290809ec3a53191dd81c7f70a4b28688a362182986f", digest);
    }

    [Fact]
    public void Hmac_SHA256_ProducesCorrectDigest()
    {
        var hmac = crypto.createHmac("sha256", "secret-key");
        hmac.update("Hello, World!");
        var digest = hmac.digest("hex");

        Assert.NotEmpty(digest);
        Assert.Equal(64, digest.Length); // SHA256 produces 32 bytes = 64 hex characters
    }

    [Fact]
    public void Cipher_Decipher_AES256_RoundTrip()
    {
        var key = crypto.randomBytes(32); // 256 bits
        var iv = crypto.randomBytes(16);  // 128 bits
        var plaintext = "Hello, World! This is a test message.";

        // Encrypt
        var cipher = crypto.createCipheriv("aes-256-cbc", key, iv);
        var encrypted = cipher.update(plaintext, "utf8", "hex");
        encrypted += cipher.final("hex");

        // Decrypt
        var decipher = crypto.createDecipheriv("aes-256-cbc", key, iv);
        var decrypted = decipher.update(encrypted, "hex", "utf8");
        decrypted += decipher.final("utf8");

        Assert.Equal(plaintext, decrypted);
    }

    [Fact]
    public void RandomBytes_GeneratesCorrectLength()
    {
        var bytes = crypto.randomBytes(32);
        Assert.Equal(32, bytes.Length);
    }

    [Fact]
    public void RandomBytes_GeneratesDifferentValues()
    {
        var bytes1 = crypto.randomBytes(16);
        var bytes2 = crypto.randomBytes(16);

        Assert.NotEqual(bytes1, bytes2);
    }

    [Fact]
    public void RandomInt_GeneratesWithinRange()
    {
        for (int i = 0; i < 100; i++)
        {
            var value = crypto.randomInt(10);
            Assert.InRange(value, 0, 9);
        }
    }

    [Fact]
    public void RandomInt_GeneratesWithinCustomRange()
    {
        for (int i = 0; i < 100; i++)
        {
            var value = crypto.randomInt(5, 15);
            Assert.InRange(value, 5, 14);
        }
    }

    [Fact]
    public void RandomUUID_GeneratesValidUUID()
    {
        var uuid = crypto.randomUUID();
        Assert.Matches(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", uuid);
    }

    [Fact]
    public void RandomFillSync_FillsBuffer()
    {
        var buffer = new byte[32];
        crypto.randomFillSync(buffer);

        // Check that buffer is not all zeros
        var allZeros = true;
        foreach (var b in buffer)
        {
            if (b != 0)
            {
                allZeros = false;
                break;
            }
        }
        Assert.False(allZeros);
    }

    [Fact]
    public void PBKDF2Sync_GeneratesCorrectLength()
    {
        var derived = crypto.pbkdf2Sync("password", "salt", 1000, 32, "sha256");
        Assert.Equal(32, derived.Length);
    }

    [Fact]
    public void PBKDF2Sync_DeterministicOutput()
    {
        var derived1 = crypto.pbkdf2Sync("password", "salt", 1000, 32, "sha256");
        var derived2 = crypto.pbkdf2Sync("password", "salt", 1000, 32, "sha256");

        Assert.Equal(derived1, derived2);
    }

    [Fact]
    public void GetCiphers_ReturnsNonEmptyList()
    {
        var ciphers = crypto.getCiphers();
        Assert.NotEmpty(ciphers);
        Assert.Contains("aes-256-cbc", ciphers);
    }

    [Fact]
    public void GetHashes_ReturnsNonEmptyList()
    {
        var hashes = crypto.getHashes();
        Assert.NotEmpty(hashes);
        Assert.Contains("sha256", hashes);
        Assert.Contains("md5", hashes);
    }

    [Fact]
    public void GetCurves_ReturnsNonEmptyList()
    {
        var curves = crypto.getCurves();
        Assert.NotEmpty(curves);
        Assert.Contains("secp256r1", curves);
    }

    [Fact]
    public void TimingSafeEqual_ReturnsTrueForEqualBuffers()
    {
        var buffer1 = Encoding.UTF8.GetBytes("Hello");
        var buffer2 = Encoding.UTF8.GetBytes("Hello");

        Assert.True(crypto.timingSafeEqual(buffer1, buffer2));
    }

    [Fact]
    public void TimingSafeEqual_ReturnsFalseForDifferentBuffers()
    {
        var buffer1 = Encoding.UTF8.GetBytes("Hello");
        var buffer2 = Encoding.UTF8.GetBytes("World");

        Assert.False(crypto.timingSafeEqual(buffer1, buffer2));
    }

    [Fact]
    public void TimingSafeEqual_ReturnsFalseForDifferentLengths()
    {
        var buffer1 = Encoding.UTF8.GetBytes("Hello");
        var buffer2 = Encoding.UTF8.GetBytes("Hello!");

        Assert.False(crypto.timingSafeEqual(buffer1, buffer2));
    }

    [Fact]
    public void CreateSecretKey_CreatesKeyObject()
    {
        var key = crypto.createSecretKey(crypto.randomBytes(32));

        Assert.Equal("secret", key.type);
        Assert.Equal(32, key.symmetricKeySize);
        Assert.Null(key.asymmetricKeyType);
    }

    [Fact]
    public void CreateECDH_GeneratesKeys()
    {
        var ecdh = crypto.createECDH("secp256r1");
        var publicKey = ecdh.generateKeys();

        Assert.NotEmpty(publicKey);
    }

    [Fact]
    public void ECDH_ComputesSharedSecret()
    {
        var alice = crypto.createECDH("secp256r1");
        var bob = crypto.createECDH("secp256r1");

        alice.generateKeys();
        bob.generateKeys();

        var aliceSecret = alice.computeSecret(bob.getPublicKey());
        var bobSecret = bob.computeSecret(alice.getPublicKey());

        Assert.Equal(aliceSecret, bobSecret);
    }

    [Fact]
    public void CreateHash_ThrowsForUnknownAlgorithm()
    {
        Assert.Throws<ArgumentException>(() => crypto.createHash("invalid-algorithm"));
    }

    [Fact]
    public void Hash_DigestOnlyOnce()
    {
        var hash = crypto.createHash("sha256");
        hash.update("test");
        hash.digest();

        Assert.Throws<InvalidOperationException>(() => hash.digest());
    }

    [Fact]
    public void Cipher_FinalOnlyOnce()
    {
        var key = crypto.randomBytes(32);
        var iv = crypto.randomBytes(16);
        var cipher = crypto.createCipheriv("aes-256-cbc", key, iv);

        cipher.update("test");
        cipher.final();

        Assert.Throws<InvalidOperationException>(() => cipher.final());
    }

    [Fact]
    public void GenerateKeyPairSync_RSA_CreatesValidKeyPair()
    {
        var (publicKey, privateKey) = crypto.generateKeyPairSync("rsa", null);

        Assert.NotNull(publicKey);
        Assert.NotNull(privateKey);
        Assert.Equal("public", publicKey.type);
        Assert.Equal("private", privateKey.type);
        Assert.Equal("rsa", publicKey.asymmetricKeyType);
        Assert.Equal("rsa", privateKey.asymmetricKeyType);
    }

    [Fact]
    public void PublicEncrypt_PrivateDecrypt_RoundTrip()
    {
        var (publicKey, privateKey) = crypto.generateKeyPairSync("rsa", null);
        var plaintext = Encoding.UTF8.GetBytes("Hello, World!");

        var encrypted = crypto.publicEncrypt(publicKey, plaintext);
        var decrypted = crypto.privateDecrypt(privateKey, encrypted);

        Assert.Equal(plaintext, decrypted);
    }

    [Fact]
    public void GetFips_ReturnsFalse()
    {
        // In .NET, FIPS mode is not directly configurable
        Assert.False(crypto.getFips());
    }
}
