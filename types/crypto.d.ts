/**
 * The crypto module provides cryptographic functionality that includes a set of
 * wrappers for OpenSSL's hash, HMAC, cipher, decipher, sign, and verify functions.
 *
 * @see [Node.js Crypto Documentation](https://nodejs.org/api/crypto.html)
 */

/// <reference types="./stream" />

type BinaryToTextEncoding = 'base64' | 'base64url' | 'hex' | 'binary' | 'latin1';
type CharacterEncoding = 'utf8' | 'utf-8' | 'utf16le' | 'utf-16le' | 'latin1' | 'binary';
type LegacyCharacterEncoding = 'ascii' | 'binary' | 'ucs2' | 'ucs-2';
type Encoding = BinaryToTextEncoding | CharacterEncoding | LegacyCharacterEncoding;

type BinaryLike = string | ArrayBufferView;
type KeyFormat = 'pem' | 'der';
type KeyType = 'pkcs1' | 'pkcs8' | 'sec1' | 'spki';

interface HashOptions {
    outputLength?: number;
}

/**
 * The Hash class is a utility for creating hash digests of data.
 */
export class Hash {
    /**
     * Updates the hash content with the given data.
     */
    update(data: BinaryLike, inputEncoding?: Encoding): this;

    /**
     * Calculates the digest of all the data passed to be hashed.
     */
    digest(): Buffer;
    digest(encoding: BinaryToTextEncoding): string;

    /**
     * Creates a copy of the Hash object in its current state.
     */
    copy(): Hash;
}

/**
 * The Hmac class is a utility for creating cryptographic HMAC digests.
 */
export class Hmac {
    /**
     * Updates the Hmac content with the given data.
     */
    update(data: BinaryLike, inputEncoding?: Encoding): this;

    /**
     * Calculates the HMAC digest of all the data passed.
     */
    digest(): Buffer;
    digest(encoding: BinaryToTextEncoding): string;
}

/**
 * Instances of the Cipher class are used to encrypt data.
 */
export class Cipher {
    /**
     * Updates the cipher with data.
     */
    update(data: BinaryLike, inputEncoding?: Encoding): Buffer;
    update(data: string, inputEncoding: Encoding | undefined, outputEncoding: Encoding): string;

    /**
     * Returns any remaining enciphered contents.
     */
    final(): Buffer;
    final(outputEncoding: BufferEncoding): string;

    /**
     * When using an authenticated encryption mode, sets the length of the authentication tag.
     */
    setAutoPadding(autoPadding?: boolean): this;

    /**
     * When using an authenticated encryption mode, returns the authentication tag.
     */
    getAuthTag(): Buffer;

    /**
     * When using an authenticated encryption mode, sets AAD (Additional Authenticated Data).
     */
    setAAD(buffer: ArrayBufferView, options?: { plaintextLength: number }): this;
}

/**
 * Instances of the Decipher class are used to decrypt data.
 */
export class Decipher {
    /**
     * Updates the decipher with data.
     */
    update(data: BinaryLike, inputEncoding?: Encoding): Buffer;
    update(data: string, inputEncoding: Encoding | undefined, outputEncoding: Encoding): string;

    /**
     * Returns any remaining deciphered contents.
     */
    final(): Buffer;
    final(outputEncoding: BufferEncoding): string;

    /**
     * When using an authenticated encryption mode, sets the authentication tag.
     */
    setAuthTag(buffer: ArrayBufferView, encoding?: string): this;

    /**
     * When using an authenticated encryption mode, sets AAD (Additional Authenticated Data).
     */
    setAAD(buffer: ArrayBufferView, options?: { plaintextLength: number }): this;

    /**
     * When using an authenticated encryption mode, disables automatic padding.
     */
    setAutoPadding(autoPadding?: boolean): this;
}

/**
 * The Sign class is a utility for generating signatures.
 */
export class Sign {
    /**
     * Updates the Sign content with the given data.
     */
    update(data: BinaryLike, inputEncoding?: Encoding): this;

    /**
     * Calculates the signature on all the data passed through using update.
     */
    sign(privateKey: KeyLike): Buffer;
    sign(privateKey: KeyLike, outputEncoding: BinaryToTextEncoding): string;
}

/**
 * The Verify class is a utility for verifying signatures.
 */
export class Verify {
    /**
     * Updates the Verify content with the given data.
     */
    update(data: BinaryLike, inputEncoding?: Encoding): this;

    /**
     * Verifies the provided data using the given public key and signature.
     */
    verify(publicKey: KeyLike, signature: BinaryLike): boolean;
    verify(publicKey: KeyLike, signature: string, signatureEncoding?: BinaryToTextEncoding): boolean;
}

/**
 * The DiffieHellman class is a utility for creating Diffie-Hellman key exchanges.
 */
export class DiffieHellman {
    /**
     * Generates private and public Diffie-Hellman key values.
     */
    generateKeys(): Buffer;
    generateKeys(encoding: BinaryToTextEncoding): string;

    /**
     * Computes the shared secret using the other party's public key.
     */
    computeSecret(otherPublicKey: BinaryLike): Buffer;
    computeSecret(otherPublicKey: BinaryLike, inputEncoding: BinaryToTextEncoding): Buffer;
    computeSecret(otherPublicKey: BinaryLike, inputEncoding: BinaryToTextEncoding, outputEncoding: BinaryToTextEncoding): string;

    /**
     * Returns the Diffie-Hellman prime.
     */
    getPrime(): Buffer;
    getPrime(encoding: BinaryToTextEncoding): string;

    /**
     * Returns the Diffie-Hellman generator.
     */
    getGenerator(): Buffer;
    getGenerator(encoding: BinaryToTextEncoding): string;

    /**
     * Returns the Diffie-Hellman public key.
     */
    getPublicKey(): Buffer;
    getPublicKey(encoding: BinaryToTextEncoding): string;

    /**
     * Returns the Diffie-Hellman private key.
     */
    getPrivateKey(): Buffer;
    getPrivateKey(encoding: BinaryToTextEncoding): string;

    /**
     * Sets the Diffie-Hellman public key.
     */
    setPublicKey(publicKey: BinaryLike, encoding?: BinaryToTextEncoding): void;

    /**
     * Sets the Diffie-Hellman private key.
     */
    setPrivateKey(privateKey: BinaryLike, encoding?: BinaryToTextEncoding): void;

    /**
     * Returns the size of the Diffie-Hellman group in bits.
     */
    getVerifyError(): number;
}

/**
 * The ECDH class is a utility for creating Elliptic Curve Diffie-Hellman (ECDH) key exchanges.
 */
export class ECDH {
    /**
     * Generates private and public EC Diffie-Hellman key values.
     */
    generateKeys(): Buffer;
    generateKeys(encoding: BinaryToTextEncoding, format?: ECDHKeyFormat): string;

    /**
     * Computes the shared secret using the other party's public key.
     */
    computeSecret(otherPublicKey: BinaryLike): Buffer;
    computeSecret(otherPublicKey: BinaryLike, inputEncoding: BinaryToTextEncoding): Buffer;
    computeSecret(otherPublicKey: BinaryLike, inputEncoding: BinaryToTextEncoding, outputEncoding: BinaryToTextEncoding): string;

    /**
     * Returns the EC Diffie-Hellman public key.
     */
    getPublicKey(): Buffer;
    getPublicKey(encoding?: BinaryToTextEncoding, format?: ECDHKeyFormat): string;

    /**
     * Returns the EC Diffie-Hellman private key.
     */
    getPrivateKey(): Buffer;
    getPrivateKey(encoding: BinaryToTextEncoding): string;

    /**
     * Sets the EC Diffie-Hellman public key.
     */
    setPublicKey(publicKey: BinaryLike, encoding?: BinaryToTextEncoding): void;

    /**
     * Sets the EC Diffie-Hellman private key.
     */
    setPrivateKey(privateKey: BinaryLike, encoding?: BinaryToTextEncoding): void;
}

type ECDHKeyFormat = 'compressed' | 'uncompressed' | 'hybrid';

/**
 * Represents a cryptographic key.
 */
export interface KeyObject {
    /**
     * The type of the key: 'secret', 'public', or 'private'.
     */
    readonly type: 'secret' | 'public' | 'private';

    /**
     * For asymmetric keys, this property returns the type of the key.
     */
    readonly asymmetricKeyType?: 'rsa' | 'rsa-pss' | 'dsa' | 'ec' | 'ed25519' | 'ed448' | 'x25519' | 'x448' | 'dh';

    /**
     * For secret keys, this property returns the size of the key in bytes.
     */
    readonly symmetricKeySize?: number;

    /**
     * Exports the key in the specified format.
     */
    export(options?: KeyExportOptions<KeyFormat>): string | Buffer;
}

interface KeyExportOptions<T extends KeyFormat> {
    type?: KeyType;
    format?: T;
    cipher?: string;
    passphrase?: string | Buffer;
}

type KeyLike = string | Buffer | KeyObject;

/**
 * Creates and returns a Hash object.
 */
export function createHash(algorithm: string, options?: HashOptions): Hash;

/**
 * Creates and returns an Hmac object.
 */
export function createHmac(algorithm: string, key: BinaryLike | KeyObject): Hmac;

/**
 * Creates and returns a Cipher object.
 */
export function createCipheriv(algorithm: string, key: BinaryLike, iv: BinaryLike | null, options?: TransformOptions): Cipher;

/**
 * Creates and returns a Decipher object.
 */
export function createDecipheriv(algorithm: string, key: BinaryLike, iv: BinaryLike | null, options?: TransformOptions): Decipher;

/**
 * Creates a Sign object.
 */
export function createSign(algorithm: string, options?: WriteableOptions): Sign;

/**
 * Creates a Verify object.
 */
export function createVerify(algorithm: string, options?: WriteableOptions): Verify;

/**
 * Creates a DiffieHellman key exchange object.
 */
export function createDiffieHellman(primeLength: number, generator?: number): DiffieHellman;
export function createDiffieHellman(prime: BinaryLike, generator?: number | BinaryLike): DiffieHellman;
export function createDiffieHellman(prime: string, primeEncoding: BinaryToTextEncoding, generator?: number | BinaryLike): DiffieHellman;
export function createDiffieHellman(prime: string, primeEncoding: BinaryToTextEncoding, generator: string, generatorEncoding: BinaryToTextEncoding): DiffieHellman;

/**
 * Gets a predefined Diffie-Hellman group.
 */
export function getDiffieHellman(groupName: string): DiffieHellman;

/**
 * Creates an ECDH key exchange object.
 */
export function createECDH(curveName: string): ECDH;

/**
 * Generates cryptographically strong pseudo-random data.
 */
export function randomBytes(size: number): Buffer;
export function randomBytes(size: number, callback: (err: Error | null, buf: Buffer) => void): void;

/**
 * Generates a random integer.
 */
export function randomInt(max: number): number;
export function randomInt(min: number, max: number): number;
export function randomInt(max: number, callback: (err: Error | null, value: number) => void): void;
export function randomInt(min: number, max: number, callback: (err: Error | null, value: number) => void): void;

/**
 * Fills a buffer with random bytes synchronously.
 */
export function randomFillSync<T extends ArrayBufferView>(buffer: T, offset?: number, size?: number): T;

/**
 * Fills a buffer with random bytes.
 */
export function randomFill<T extends ArrayBufferView>(buffer: T, callback: (err: Error | null, buf: T) => void): void;
export function randomFill<T extends ArrayBufferView>(buffer: T, offset: number, callback: (err: Error | null, buf: T) => void): void;
export function randomFill<T extends ArrayBufferView>(buffer: T, offset: number, size: number, callback: (err: Error | null, buf: T) => void): void;

/**
 * Generates a UUID v4.
 */
export function randomUUID(): string;

/**
 * Provides a synchronous Password-Based Key Derivation Function 2 (PBKDF2) implementation.
 */
export function pbkdf2Sync(password: BinaryLike, salt: BinaryLike, iterations: number, keylen: number, digest: string): Buffer;

/**
 * Provides an asynchronous Password-Based Key Derivation Function 2 (PBKDF2) implementation.
 */
export function pbkdf2(password: BinaryLike, salt: BinaryLike, iterations: number, keylen: number, digest: string, callback: (err: Error | null, derivedKey: Buffer) => void): void;

/**
 * Provides a synchronous scrypt implementation.
 */
export function scryptSync(password: BinaryLike, salt: BinaryLike, keylen: number, options?: ScryptOptions): Buffer;

/**
 * Provides an asynchronous scrypt implementation.
 */
export function scrypt(password: BinaryLike, salt: BinaryLike, keylen: number, callback: (err: Error | null, derivedKey: Buffer) => void): void;
export function scrypt(password: BinaryLike, salt: BinaryLike, keylen: number, options: ScryptOptions, callback: (err: Error | null, derivedKey: Buffer) => void): void;

interface ScryptOptions {
    cost?: number;
    blockSize?: number;
    parallelization?: number;
    N?: number;
    r?: number;
    p?: number;
    maxmem?: number;
}

/**
 * Derives a key using the HKDF algorithm.
 */
export function hkdfSync(digest: string, ikm: BinaryLike, salt: BinaryLike, info: BinaryLike, keylen: number): Buffer;

/**
 * Derives a key using the HKDF algorithm (async).
 */
export function hkdf(digest: string, ikm: BinaryLike, salt: BinaryLike, info: BinaryLike, keylen: number, callback: (err: Error | null, derivedKey: Buffer) => void): void;

/**
 * Returns an array of the names of the supported cipher algorithms.
 */
export function getCiphers(): string[];

/**
 * Returns an array of the names of the supported hash algorithms.
 */
export function getHashes(): string[];

/**
 * Returns an array of the names of the supported elliptic curves.
 */
export function getCurves(): string[];

/**
 * Returns the default cipher list.
 */
export function getDefaultCipherList(): string;

/**
 * Test for equality in constant time.
 */
export function timingSafeEqual(a: ArrayBufferView, b: ArrayBufferView): boolean;

/**
 * Creates a KeyObject from a secret key.
 */
export function createSecretKey(key: ArrayBufferView): KeyObject;
export function createSecretKey(key: string, encoding: BufferEncoding): KeyObject;

/**
 * Creates a public KeyObject from a key.
 */
export function createPublicKey(key: string | Buffer | KeyObject): KeyObject;

/**
 * Creates a private KeyObject from a key.
 */
export function createPrivateKey(key: string | Buffer | KeyObject): KeyObject;

/**
 * Generates a new asymmetric key pair.
 */
export function generateKeyPair(type: 'rsa' | 'dsa', options: RSAKeyPairOptions<'pem', 'pem'>, callback: (err: Error | null, publicKey: string, privateKey: string) => void): void;
export function generateKeyPair(type: 'rsa' | 'dsa', options: RSAKeyPairOptions<'pem', 'der'>, callback: (err: Error | null, publicKey: string, privateKey: Buffer) => void): void;
export function generateKeyPair(type: 'rsa' | 'dsa', options: RSAKeyPairOptions<'der', 'pem'>, callback: (err: Error | null, publicKey: Buffer, privateKey: string) => void): void;
export function generateKeyPair(type: 'rsa' | 'dsa', options: RSAKeyPairOptions<'der', 'der'>, callback: (err: Error | null, publicKey: Buffer, privateKey: Buffer) => void): void;
export function generateKeyPair(type: 'rsa' | 'dsa', options: RSAKeyPairKeyObjectOptions, callback: (err: Error | null, publicKey: KeyObject, privateKey: KeyObject) => void): void;
export function generateKeyPair(type: 'ec', options: ECKeyPairOptions<'pem', 'pem'>, callback: (err: Error | null, publicKey: string, privateKey: string) => void): void;
export function generateKeyPair(type: 'ec', options: ECKeyPairOptions<'pem', 'der'>, callback: (err: Error | null, publicKey: string, privateKey: Buffer) => void): void;
export function generateKeyPair(type: 'ec', options: ECKeyPairOptions<'der', 'pem'>, callback: (err: Error | null, publicKey: Buffer, privateKey: string) => void): void;
export function generateKeyPair(type: 'ec', options: ECKeyPairOptions<'der', 'der'>, callback: (err: Error | null, publicKey: Buffer, privateKey: Buffer) => void): void;
export function generateKeyPair(type: 'ec', options: ECKeyPairKeyObjectOptions, callback: (err: Error | null, publicKey: KeyObject, privateKey: KeyObject) => void): void;

/**
 * Generates a new asymmetric key pair synchronously.
 */
export function generateKeyPairSync(type: 'rsa' | 'dsa', options: RSAKeyPairOptions<'pem', 'pem'>): { publicKey: string; privateKey: string };
export function generateKeyPairSync(type: 'rsa' | 'dsa', options: RSAKeyPairOptions<'pem', 'der'>): { publicKey: string; privateKey: Buffer };
export function generateKeyPairSync(type: 'rsa' | 'dsa', options: RSAKeyPairOptions<'der', 'pem'>): { publicKey: Buffer; privateKey: string };
export function generateKeyPairSync(type: 'rsa' | 'dsa', options: RSAKeyPairOptions<'der', 'der'>): { publicKey: Buffer; privateKey: Buffer };
export function generateKeyPairSync(type: 'rsa' | 'dsa', options: RSAKeyPairKeyObjectOptions): KeyPairKeyObjectResult;
export function generateKeyPairSync(type: 'ec', options: ECKeyPairOptions<'pem', 'pem'>): { publicKey: string; privateKey: string };
export function generateKeyPairSync(type: 'ec', options: ECKeyPairOptions<'pem', 'der'>): { publicKey: string; privateKey: Buffer };
export function generateKeyPairSync(type: 'ec', options: ECKeyPairOptions<'der', 'pem'>): { publicKey: Buffer; privateKey: string };
export function generateKeyPairSync(type: 'ec', options: ECKeyPairOptions<'der', 'der'>): { publicKey: Buffer; privateKey: Buffer };
export function generateKeyPairSync(type: 'ec', options: ECKeyPairKeyObjectOptions): KeyPairKeyObjectResult;

interface BasePrivateKeyEncodingOptions<T extends KeyFormat> {
    format: T;
    cipher?: string;
    passphrase?: string;
}

interface KeyPairKeyObjectResult {
    publicKey: KeyObject;
    privateKey: KeyObject;
}

interface RSAKeyPairOptions<PubF extends KeyFormat, PrivF extends KeyFormat> {
    modulusLength: number;
    publicExponent?: number;
    publicKeyEncoding: { type: 'pkcs1' | 'spki'; format: PubF };
    privateKeyEncoding: BasePrivateKeyEncodingOptions<PrivF> & { type: 'pkcs1' | 'pkcs8' };
}

interface RSAKeyPairKeyObjectOptions {
    modulusLength: number;
    publicExponent?: number;
}

interface ECKeyPairOptions<PubF extends KeyFormat, PrivF extends KeyFormat> {
    namedCurve: string;
    publicKeyEncoding: { type: 'spki'; format: PubF };
    privateKeyEncoding: BasePrivateKeyEncodingOptions<PrivF> & { type: 'sec1' | 'pkcs8' };
}

interface ECKeyPairKeyObjectOptions {
    namedCurve: string;
}

/**
 * Encrypts data with a public key.
 */
export function publicEncrypt(key: KeyLike, buffer: ArrayBufferView): Buffer;

/**
 * Decrypts data with a private key.
 */
export function privateDecrypt(key: KeyLike, buffer: ArrayBufferView): Buffer;

/**
 * Decrypts data with a public key.
 */
export function publicDecrypt(key: KeyLike, buffer: ArrayBufferView): Buffer;

/**
 * Encrypts data with a private key.
 */
export function privateEncrypt(key: KeyLike, buffer: ArrayBufferView): Buffer;

/**
 * Signs data using a private key.
 */
export function sign(algorithm: string | null, data: ArrayBufferView, key: KeyLike): Buffer;

/**
 * Verifies a signature using a public key.
 */
export function verify(algorithm: string | null, data: ArrayBufferView, key: KeyLike, signature: ArrayBufferView): boolean;

/**
 * Gets the fips mode status.
 */
export function getFips(): boolean;

/**
 * Sets the fips mode.
 */
export function setFips(enabled: boolean): void;

/**
 * Sets the default encoding for crypto operations.
 */
export function setDefaultEncoding(encoding: string): void;
