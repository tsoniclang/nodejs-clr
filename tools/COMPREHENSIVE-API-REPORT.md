# COMPREHENSIVE TSONIC.NODE API ANALYSIS REPORT
Generated: 2025-11-06T13:39:54.492Z

Total Types/Modules: 128

---

## PART 1: STATIC MODULES (Node.js-style APIs)

Found 21 static modules


### crypto
**Methods**: 63
**Properties**: 0

**Method Signatures:**

- `createCipheriv(algorithm: string, key: Byte[], iv: Byte[]): Cipher`
- `createCipheriv(algorithm: string, key: string, iv: string): Cipher`
- `createDecipheriv(algorithm: string, key: Byte[], iv: Byte[]): Decipher`
- `createDecipheriv(algorithm: string, key: string, iv: string): Decipher`
- `createDiffieHellman(primeLength: number, generator?: number): DiffieHellman`
- `createDiffieHellman(prime: Byte[], generator: Byte[]): DiffieHellman`
- `createDiffieHellman(prime: Byte[], generator?: number): DiffieHellman`
- `createDiffieHellman(prime: string, primeEncoding: string, generator?: number): DiffieHellman`
- `createDiffieHellman(prime: string, primeEncoding: string, generator: string, generatorEncoding: string): DiffieHellman`
- `createECDH(curveName: string): ECDH`
- `createHash(algorithm: string): Hash`
- `createHmac(algorithm: string, key: string): Hmac`
- `createHmac(algorithm: string, key: Byte[]): Hmac`
- `createPrivateKey(key: string): KeyObject`
- `createPrivateKey(key: Byte[]): KeyObject`
- `createPublicKey(key: string): KeyObject`
- `createPublicKey(key: Byte[]): KeyObject`
- `createPublicKey(key: KeyObject): KeyObject`
- `createSecretKey(key: Byte[]): KeyObject`
- `createSecretKey(key: string, encoding?: string): KeyObject`
- `createSign(algorithm: string): Sign`
- `createVerify(algorithm: string): Verify`
- `generateKey(type: string, options: any): KeyObject`
- `generateKey(type: string, options: any, callback: Action`2): void`
- `generateKeyPair(type: string, options: any, callback: Action`3): void`
- `generateKeyPairSync(type: string, options?: any): ValueTuple`2`
- `getCiphers(): string[]`
- `getCurves(): string[]`
- `getDefaultCipherList(): string`
- `getDiffieHellman(groupName: string): DiffieHellman`
- `getFips(): boolean`
- `getHashes(): string[]`
- `hash(algorithm: string, data: Byte[], outputEncoding?: string): Byte[]`
- `hkdf(digest: string, ikm: Byte[], salt: Byte[], info: Byte[], keylen: number, callback: Action`2): void`
- `hkdfSync(digest: string, ikm: Byte[], salt: Byte[], info: Byte[], keylen: number): Byte[]`
- `pbkdf2(password: string, salt: string, iterations: number, keylen: number, digest: string, callback: Action`2): void`
- `pbkdf2Sync(password: string, salt: string, iterations: number, keylen: number, digest: string): Byte[]`
- `pbkdf2Sync(password: Byte[], salt: Byte[], iterations: number, keylen: number, digest: string): Byte[]`
- `privateDecrypt(key: string, buffer: Byte[]): Byte[]`
- `privateDecrypt(key: any, buffer: Byte[]): Byte[]`
- `privateEncrypt(key: string, buffer: Byte[]): Byte[]`
- `privateEncrypt(key: any, buffer: Byte[]): Byte[]`
- `publicDecrypt(key: string, buffer: Byte[]): Byte[]`
- `publicDecrypt(key: any, buffer: Byte[]): Byte[]`
- `publicEncrypt(key: string, buffer: Byte[]): Byte[]`
- `publicEncrypt(key: any, buffer: Byte[]): Byte[]`
- `randomBytes(size: number): Byte[]`
- `randomBytes(size: number, callback: Action`2): void`
- `randomFill(buffer: Byte[], offset: number, size: number, callback: Action`2): void`
- `randomFillSync(buffer: Byte[], offset?: number, size?: number | null): Byte[]`
- `randomInt(max: number): number`
- `randomInt(min: number, max: number): number`
- `randomUUID(): string`
- `scrypt(password: string, salt: string, keylen: number, options: any, callback: Action`2): void`
- `scryptSync(password: string, salt: string, keylen: number, options?: any): Byte[]`
- `scryptSync(password: Byte[], salt: Byte[], keylen: number, options?: any): Byte[]`
- `setDefaultEncoding(encoding: string): void`
- `setFips(enabled: boolean): void`
- `sign(algorithm: string, data: Byte[], privateKey: string): Byte[]`
- `sign(algorithm: string, data: Byte[], privateKey: KeyObject): Byte[]`
- `timingSafeEqual(a: Byte[], b: Byte[]): boolean`
- `verify(algorithm: string, data: Byte[], publicKey: string, signature: Byte[]): boolean`
- `verify(algorithm: string, data: Byte[], publicKey: KeyObject, signature: Byte[]): boolean`

### fs
**Methods**: 53
**Properties**: 0

**Method Signatures:**

- `access(path: string, mode?: number): Task`
- `accessSync(path: string, mode?: number): void`
- `appendFile(path: string, data: string, encoding?: string): Task`
- `appendFileSync(path: string, data: string, encoding?: string): void`
- `chmod(path: string, mode: number): Task`
- `chmodSync(path: string, mode: number): void`
- `close(fd: number): Task`
- `closeSync(fd: number): void`
- `copyFile(src: string, dest: string, mode?: number): Task`
- `copyFileSync(src: string, dest: string, mode?: number): void`
- `cp(src: string, dest: string, recursive?: boolean): Task`
- `cpSync(src: string, dest: string, recursive?: boolean): void`
- `existsSync(path: string): boolean`
- `fstat(fd: number): Task`1`
- `fstatSync(fd: number): Stats`
- `mkdir(path: string, recursive?: boolean): Task`
- `mkdirSync(path: string, recursive?: boolean): void`
- `open(path: string, flags: string, mode?: number | null): Task`1`
- `openSync(path: string, flags: string, mode?: number | null): number`
- `read(fd: number, buffer: Byte[], offset: number, length: number, position: number | null): Task`1`
- `readFile(path: string, encoding?: string): Task`1`
- `readFileBytes(path: string): Task`1`
- `readFileSync(path: string, encoding?: string): string`
- `readFileSyncBytes(path: string): Byte[]`
- `readSync(fd: number, buffer: Byte[], offset: number, length: number, position: number | null): number`
- `readdir(path: string, withFileTypes?: boolean): Task`1`
- `readdirSync(path: string, withFileTypes?: boolean): string[]`
- `readlink(path: string): Task`1`
- `readlinkSync(path: string): string`
- `realpath(path: string): Task`1`
- `realpathSync(path: string): string`
- `rename(oldPath: string, newPath: string): Task`
- `renameSync(oldPath: string, newPath: string): void`
- `rm(path: string, recursive?: boolean): Task`
- `rmSync(path: string, recursive?: boolean): void`
- `rmdir(path: string, recursive?: boolean): Task`
- `rmdirSync(path: string, recursive?: boolean): void`
- `stat(path: string): Task`1`
- `statSync(path: string): Stats`
- `symlink(target: string, path: string, type?: string): Task`
- `symlinkSync(target: string, path: string, type?: string): void`
- `truncate(path: string, len?: Int64): Task`
- `truncateSync(path: string, len?: Int64): void`
- `unlink(path: string): Task`
- `unlinkSync(path: string): void`
- `write(fd: number, buffer: Byte[], offset: number, length: number, position: number | null): Task`1`
- `write(fd: number, data: string, position?: number | null, encoding?: string): Task`1`
- `writeFile(path: string, data: string, encoding?: string): Task`
- `writeFileBytes(path: string, data: Byte[]): Task`
- `writeFileSync(path: string, data: string, encoding?: string): void`
- `writeFileSyncBytes(path: string, data: Byte[]): void`
- `writeSync(fd: number, buffer: Byte[], offset: number, length: number, position: number | null): number`
- `writeSync(fd: number, data: string, position?: number | null, encoding?: string): number`

### dns
**Methods**: 27
**Properties**: 0

**Method Signatures:**

- `getDefaultResultOrder(): string`
- `getServers(): string[]`
- `lookup(hostname: string, callback: Action`3): void`
- `lookup(hostname: string, family: number, callback: Action`3): void`
- `lookup(hostname: string, options: LookupOptions, callback: Action`3): void`
- `lookup(hostname: string, options: LookupOptions, callback: Action`2): void`
- `lookupService(address: string, port: number, callback: Action`3): void`
- `resolve(hostname: string, callback: Action`2): void`
- `resolve(hostname: string, rrtype: string, callback: Action`2): void`
- `resolve4(hostname: string, callback: Action`2): void`
- `resolve4(hostname: string, options: ResolveOptions, callback: Action`2): void`
- `resolve6(hostname: string, callback: Action`2): void`
- `resolve6(hostname: string, options: ResolveOptions, callback: Action`2): void`
- `resolveAny(hostname: string, callback: Action`2): void`
- `resolveCaa(hostname: string, callback: Action`2): void`
- `resolveCname(hostname: string, callback: Action`2): void`
- `resolveMx(hostname: string, callback: Action`2): void`
- `resolveNaptr(hostname: string, callback: Action`2): void`
- `resolveNs(hostname: string, callback: Action`2): void`
- `resolvePtr(hostname: string, callback: Action`2): void`
- `resolveSoa(hostname: string, callback: Action`2): void`
- `resolveSrv(hostname: string, callback: Action`2): void`
- `resolveTlsa(hostname: string, callback: Action`2): void`
- `resolveTxt(hostname: string, callback: Action`2): void`
- `reverse(ip: string, callback: Action`2): void`
- `setDefaultResultOrder(order: string): void`
- `setServers(servers: string[]): void`

### console
**Methods**: 22
**Properties**: 0

**Method Signatures:**

- `assert(value: boolean, message?: string, optionalParams: any[]): void`
- `clear(): void`
- `count(label?: string): void`
- `countReset(label?: string): void`
- `debug(message?: any, optionalParams: any[]): void`
- `dir(obj: any, options: any[]): void`
- `dirxml(data: any[]): void`
- `error(message?: any, optionalParams: any[]): void`
- `group(label: any[]): void`
- `groupCollapsed(label: any[]): void`
- `groupEnd(): void`
- `info(message?: any, optionalParams: any[]): void`
- `log(message?: any, optionalParams: any[]): void`
- `profile(label?: string): void`
- `profileEnd(label?: string): void`
- `table(tabularData: any, properties?: string[]): void`
- `time(label?: string): void`
- `timeEnd(label?: string): void`
- `timeLog(label?: string, data: any[]): void`
- `timeStamp(label?: string): void`
- `trace(message?: any, optionalParams: any[]): void`
- `warn(message?: any, optionalParams: any[]): void`

### assert
**Methods**: 15
**Properties**: 0

**Method Signatures:**

- `deepEqual(actual: any, expected: any, message?: string): void`
- `deepStrictEqual(actual: any, expected: any, message?: string): void`
- `doesNotMatch(string: string, regexp: Regex, message?: string): void`
- `doesNotThrow(fn: Action, message?: string): void`
- `equal(actual: any, expected: any, message?: string): void`
- `fail(message?: string): void`
- `ifError(value: any): void`
- `match(string: string, regexp: Regex, message?: string): void`
- `notDeepEqual(actual: any, expected: any, message?: string): void`
- `notDeepStrictEqual(actual: any, expected: any, message?: string): void`
- `notEqual(actual: any, expected: any, message?: string): void`
- `notStrictEqual(actual: any, expected: any, message?: string): void`
- `ok(value: boolean, message?: string): void`
- `strictEqual(actual: any, expected: any, message?: string): void`
- `throws(fn: Action, message?: string): void`

### net
**Methods**: 15
**Properties**: 0

**Method Signatures:**

- `connect(port: number, host?: string, connectionListener?: Action): Socket`
- `connect(options: TcpSocketConnectOpts, connectionListener?: Action): Socket`
- `connect(path: string, connectionListener?: Action): Socket`
- `createConnection(port: number, host?: string, connectionListener?: Action): Socket`
- `createConnection(options: TcpSocketConnectOpts, connectionListener?: Action): Socket`
- `createConnection(path: string, connectionListener?: Action): Socket`
- `createServer(connectionListener?: (arg0: Socket) => void): Server`
- `createServer(options: ServerOpts, connectionListener?: (arg0: Socket) => void): Server`
- `getDefaultAutoSelectFamily(): boolean`
- `getDefaultAutoSelectFamilyAttemptTimeout(): number`
- `isIP(input: string): number`
- `isIPv4(input: string): boolean`
- `isIPv6(input: string): boolean`
- `setDefaultAutoSelectFamily(value: boolean): void`
- `setDefaultAutoSelectFamilyAttemptTimeout(value: number): void`

### os
**Methods**: 15
**Properties**: 0

**Method Signatures:**

- `arch(): string`
- `availableParallelism(): number`
- `cpus(): CpuInfo[]`
- `endianness(): string`
- `freemem(): Int64`
- `homedir(): string`
- `hostname(): string`
- `loadavg(): number[]`
- `platform(): string`
- `release(): string`
- `tmpdir(): string`
- `totalmem(): Int64`
- `type(): string`
- `uptime(): Int64`
- `userInfo(): UserInfo`

### path
**Methods**: 12
**Properties**: 2

**Method Signatures:**

- `basename(path: string, suffix?: string): string`
- `dirname(path: string): string`
- `extname(path: string): string`
- `format(pathObject: ParsedPath): string`
- `isAbsolute(path: string): boolean`
- `join(paths: string[]): string`
- `matchesGlob(path: string, pattern: string): boolean`
- `normalize(path: string): string`
- `parse(path: string): ParsedPath`
- `relative(from: string, to: string): string`
- `resolve(paths: string[]): string`
- `toNamespacedPath(path: string): string`

**Properties:**

- `posix: PathModule` (readonly)
- `win32: PathModule` (readonly)

### zlib
**Methods**: 11
**Properties**: 0

**Method Signatures:**

- `brotliCompressSync(buffer: Byte[], options?: BrotliOptions): Byte[]`
- `brotliDecompressSync(buffer: Byte[], options?: BrotliOptions): Byte[]`
- `crc32(data: Byte[], value?: UInt32): UInt32`
- `crc32(data: string, value?: UInt32): UInt32`
- `deflateRawSync(buffer: Byte[], options?: ZlibOptions): Byte[]`
- `deflateSync(buffer: Byte[], options?: ZlibOptions): Byte[]`
- `gunzipSync(buffer: Byte[], options?: ZlibOptions): Byte[]`
- `gzipSync(buffer: Byte[], options?: ZlibOptions): Byte[]`
- `inflateRawSync(buffer: Byte[], options?: ZlibOptions): Byte[]`
- `inflateSync(buffer: Byte[], options?: ZlibOptions): Byte[]`
- `unzipSync(buffer: Byte[], options?: ZlibOptions): Byte[]`

### child_process
**Methods**: 10
**Properties**: 0

**Method Signatures:**

- `exec(command: string, options: ExecOptions, callback: Action`3): void`
- `exec(command: string, callback: Action`3): void`
- `execFile(file: string, args: string[], options: ExecOptions, callback: Action`3): void`
- `execFileSync(file: string, args?: string[], options?: ExecOptions): any`
- `execSync(command: string): Byte[]`
- `execSync(command: string, options: ExecOptions): any`
- `fork(modulePath: string, args?: string[], options?: ExecOptions): ChildProcess`
- `spawn(command: string, args?: string[], options?: ExecOptions): ChildProcess`
- `spawnSync(command: string, args?: string[], options?: ExecOptions): SpawnSyncReturns`1`
- `spawnSyncString(command: string, args?: string[], options?: ExecOptions): SpawnSyncReturns`1`

### tls
**Methods**: 10
**Properties**: 0

**Method Signatures:**

- `checkServerIdentity(hostname: string, cert: PeerCertificate): Exception`
- `connect(options: ConnectionOptions, secureConnectListener?: Action): TLSSocket`
- `connect(port: number, host?: string, options?: ConnectionOptions, secureConnectListener?: Action): TLSSocket`
- `connect(port: number, options?: ConnectionOptions, secureConnectListener?: Action): TLSSocket`
- `createSecureContext(options?: SecureContextOptions): SecureContext`
- `createServer(secureConnectionListener?: (arg0: TLSSocket) => void): TLSServer`
- `createServer(options: TlsOptions, secureConnectionListener?: (arg0: TLSSocket) => void): TLSServer`
- `getCACertificates(type?: string): string[]`
- `getCiphers(): string[]`
- `setDefaultCACertificates(certs: string[]): void`

### performance
**Methods**: 8
**Properties**: 0

**Method Signatures:**

- `clearMarks(name?: string): void`
- `clearMeasures(name?: string): void`
- `getEntries(): PerformanceEntry[]`
- `getEntriesByName(name: string, type?: string): PerformanceEntry[]`
- `getEntriesByType(type: string): PerformanceEntry[]`
- `mark(name: string, options?: MarkOptions): PerformanceMark`
- `measure(name: string, startOrOptions?: any, endMark?: string): PerformanceMeasure`
- `now(): number`

### util
**Methods**: 8
**Properties**: 0

**Method Signatures:**

- `debuglog(section: string): DebugLogFunction`
- `deprecate(fn: () => TResult, msg: string, code?: string): () => TResult`
- `deprecate(action: Action, msg: string, code?: string): Action`
- `format(format: any, args: any[]): string`
- `inherits(constructor: any, superConstructor: any): void`
- `inspect(obj: any): string`
- `isArray(obj: any): boolean`
- `isDeepStrictEqual(val1: any, val2: any): boolean`

### readline
**Methods**: 7
**Properties**: 0

**Method Signatures:**

- `clearLine(stream: Writable, dir: number, callback?: Action): boolean`
- `clearScreenDown(stream: Writable, callback?: Action): boolean`
- `createAsyncIterator(input: Readable, options?: InterfaceOptions): IAsyncEnumerable`1`
- `createInterface(options: InterfaceOptions): Interface`
- `createInterface(input: Readable, output?: Writable): Interface`
- `cursorTo(stream: Writable, x: number, y?: number | null, callback?: Action): boolean`
- `moveCursor(stream: Writable, dx: number, dy: number, callback?: Action): boolean`

### timers
**Methods**: 7
**Properties**: 0

**Method Signatures:**

- `clearImmediate(immediate: Immediate): void`
- `clearInterval(timeout: Timeout): void`
- `clearTimeout(timeout: Timeout): void`
- `queueMicrotask(callback: Action): void`
- `setImmediate(callback: Action): Immediate`
- `setInterval(callback: Action, delay?: number): Timeout`
- `setTimeout(callback: Action, delay?: number): Timeout`

### Certificate
**Methods**: 6
**Properties**: 0

**Method Signatures:**

- `exportChallenge(spkac: string): Byte[]`
- `exportChallenge(spkac: Byte[]): Byte[]`
- `exportPublicKey(spkac: string): Byte[]`
- `exportPublicKey(spkac: Byte[]): Byte[]`
- `verifySpkac(spkac: string): boolean`
- `verifySpkac(spkac: Byte[]): boolean`

### querystring
**Methods**: 6
**Properties**: 0

**Method Signatures:**

- `decode(str: string, sep?: string, eq?: string, maxKeys?: number): Record<string, any>`
- `encode(obj: Record<string, any>, sep?: string, eq?: string): string`
- `escape(str: string): string`
- `parse(str: string, sep?: string, eq?: string, maxKeys?: number): Record<string, any>`
- `stringify(obj: Record<string, any>, sep?: string, eq?: string): string`
- `unescape(str: string): string`

### process
**Methods**: 4
**Properties**: 11

**Method Signatures:**

- `chdir(directory: string): void`
- `cwd(): string`
- `exit(code?: number | null): void`
- `kill(pid: number, signal?: any): boolean`

**Properties:**

- `arch: string` (readonly)
- `argv: string[]` (get/set)
- `argv0: string` (get/set)
- `env: ProcessEnv` (readonly)
- `execPath: string` (readonly)
- `exitCode: number | null` (get/set)
- `pid: number` (readonly)
- `platform: string` (readonly)
- `ppid: number` (readonly)
- `version: string` (readonly)
- `versions: ProcessVersions` (readonly)

### stream
**Methods**: 3
**Properties**: 0

**Method Signatures:**

- `finished(stream: Stream, callback: (arg0: Exception) => void): void`
- `finished(stream: Stream): Task`
- `pipeline(streams: any[]): void`

### X509CertificateExtensions
**Methods**: 2
**Properties**: 0

**Method Signatures:**

- `ParseCertificate(certificate: string): X509CertificateInfo`
- `ParseCertificate(certificate: Byte[]): X509CertificateInfo`

### dgram
**Methods**: 2
**Properties**: 0

**Method Signatures:**

- `createSocket(type: string, callback?: Action`2): DgramSocket`
- `createSocket(options: SocketOptions, callback?: Action`2): DgramSocket`


---

## PART 2: INSTANCE CLASSES

Found 107 instance classes


### Buffer
**Methods**: 91 | **Properties**: 3

**Methods:** (91 total - listing first 10)

- `alloc(size: number, fill?: any, encoding?: string): Buffer`
- `allocUnsafe(size: number): Buffer`
- `allocUnsafeSlow(size: number): Buffer`
- `byteLength(str: string, encoding?: string): number`
- `compare(target: Buffer, targetStart?: number | null, targetEnd?: number | null, sourceStart?: number | null, sourceEnd?: number | null): number`
- `compare(buf1: Buffer, buf2: Buffer): number`
- `concat(list: Buffer[], totalLength?: number | null): Buffer`
- `copy(target: Buffer, targetStart?: number, sourceStart?: number | null, sourceEnd?: number | null): number`
- `equals(otherBuffer: Buffer): boolean`
- `fill(value: any, offset?: number, end?: number | null, encoding?: string): Buffer`
- ... and 81 more methods

**Properties:**

- `Item: Byte` (get/set)
- `length: number` (readonly)
- `poolSize: number` (get/set)

### DgramSocket
**Methods**: 36 | **Properties**: 0

**Methods:** (36 total - listing first 10)

- `addMembership(multicastAddress: string, multicastInterface?: string): void`
- `addSourceSpecificMembership(sourceAddress: string, groupAddress: string, multicastInterface?: string): void`
- `address(): AddressInfo`
- `bind(port?: number, address?: string, callback?: Action): DgramSocket`
- `bind(port: number, callback: Action): DgramSocket`
- `bind(callback: Action): DgramSocket`
- `bind(options: BindOptions, callback?: Action): DgramSocket`
- `close(callback?: Action): DgramSocket`
- `connect(port: number, address?: string, callback?: Action): void`
- `connect(port: number, callback: Action): void`
- ... and 26 more methods

### Socket
**Methods**: 20 | **Properties**: 11

**Methods:**

- `address(): any`
- `connect(port: number, host?: string, connectionListener?: Action): Socket`
- `connect(options: TcpSocketConnectOpts, connectionListener?: Action): Socket`
- `connect(path: string, connectionListener?: Action): Socket`
- `destroy(error?: Exception): Socket`
- `destroySoon(): void`
- `end(callback?: Action): Socket`
- `end(data: Byte[], callback?: Action): Socket`
- `end(data: string, encoding?: string, callback?: Action): Socket`
- `pause(): Socket`
- `ref(): Socket`
- `resetAndDestroy(): Socket`
- `resume(): Socket`
- `setEncoding(encoding?: string): Socket`
- `setKeepAlive(enable?: boolean, initialDelay?: number): Socket`
- `setNoDelay(noDelay?: boolean): Socket`
- `setTimeout(timeout: number, callback?: Action): Socket`
- `unref(): Socket`
- `write(data: Byte[], callback?: (arg0: Exception) => void): boolean`
- `write(data: string, encoding?: string, callback?: (arg0: Exception) => void): boolean`

**Properties:**

- `bytesRead: Int64` (get/set)
- `bytesWritten: Int64` (get/set)
- `connecting: boolean` (readonly)
- `destroyed: boolean` (readonly)
- `localAddress: string` (get/set)
- `localFamily: string` (get/set)
- `localPort: number | null` (get/set)
- `readyState: string` (readonly)
- `remoteAddress: string` (get/set)
- `remoteFamily: string` (get/set)
- `remotePort: number | null` (get/set)

### TLSSocket
**Methods**: 22 | **Properties**: 4

**Methods:** (22 total - listing first 10)

- `StartReading(): void`
- `disableRenegotiation(): void`
- `enableTrace(): void`
- `exportKeyingMaterial(length: number, label: string, context: Byte[]): Byte[]`
- `getCertificate(): PeerCertificate`
- `getCipher(): CipherNameAndProtocol`
- `getEphemeralKeyInfo(): EphemeralKeyInfo`
- `getFinished(): Byte[]`
- `getPeerCertificate(detailed?: boolean): PeerCertificate`
- `getPeerFinished(): Byte[]`
- ... and 12 more methods

**Properties:**

- `alpnProtocol: string` (readonly)
- `authorizationError: Exception` (readonly)
- `authorized: boolean` (readonly)
- `encrypted: boolean` (readonly)

### Resolver
**Methods**: 22 | **Properties**: 0

**Methods:** (22 total - listing first 10)

- `cancel(): void`
- `getServers(): string[]`
- `resolve(hostname: string, callback: Action`2): void`
- `resolve(hostname: string, rrtype: string, callback: Action`2): void`
- `resolve4(hostname: string, callback: Action`2): void`
- `resolve4(hostname: string, options: ResolveOptions, callback: Action`2): void`
- `resolve6(hostname: string, callback: Action`2): void`
- `resolve6(hostname: string, options: ResolveOptions, callback: Action`2): void`
- `resolveAny(hostname: string, callback: Action`2): void`
- `resolveCaa(hostname: string, callback: Action`2): void`
- ... and 12 more methods

### DiffieHellman
**Methods**: 19 | **Properties**: 0

**Methods:**

- `Dispose(): void`
- `computeSecret(otherPublicKey: string, inputEncoding?: string, outputEncoding?: string): string`
- `computeSecret(otherPublicKey: Byte[], outputEncoding?: string): string`
- `computeSecret(otherPublicKey: Byte[]): Byte[]`
- `generateKeys(encoding?: string): string`
- `generateKeys(): Byte[]`
- `getGenerator(encoding?: string): string`
- `getGenerator(): Byte[]`
- `getPrime(encoding?: string): string`
- `getPrime(): Byte[]`
- `getPrivateKey(encoding?: string): string`
- `getPrivateKey(): Byte[]`
- `getPublicKey(encoding?: string): string`
- `getPublicKey(): Byte[]`
- `getVerifyError(): number`
- `setPrivateKey(privateKey: string, encoding?: string): void`
- `setPrivateKey(privateKey: Byte[]): void`
- `setPublicKey(publicKey: string, encoding?: string): void`
- `setPublicKey(publicKey: Byte[]): void`

### EventEmitter
**Methods**: 16 | **Properties**: 1

**Methods:**

- `addListener(eventName: string, listener: Delegate): EventEmitter`
- `emit(eventName: string, args: any[]): boolean`
- `eventNames(): string[]`
- `getMaxListeners(): number`
- `listenerCount(eventName: string): number`
- `listeners(eventName: string): Delegate[]`
- `off(eventName: string, listener: Delegate): EventEmitter`
- `on(eventName: string, listener: Delegate): EventEmitter`
- `once(emitter: EventEmitter, eventName: string): Task`1`
- `once(eventName: string, listener: Delegate): EventEmitter`
- `prependListener(eventName: string, listener: Delegate): EventEmitter`
- `prependOnceListener(eventName: string, listener: Delegate): EventEmitter`
- `rawListeners(eventName: string): Delegate[]`
- `removeAllListeners(eventName?: string): EventEmitter`
- `removeListener(eventName: string, listener: Delegate): EventEmitter`
- `setMaxListeners(n: number): EventEmitter`

**Properties:**

- `defaultMaxListeners: number` (get/set)

### PathModule
**Methods**: 12 | **Properties**: 5

**Methods:**

- `basename(path: string, suffix?: string): string`
- `dirname(path: string): string`
- `extname(path: string): string`
- `format(pathObject: ParsedPath): string`
- `isAbsolute(path: string): boolean`
- `join(paths: string[]): string`
- `matchesGlob(path: string, pattern: string): boolean`
- `normalize(path: string): string`
- `parse(path: string): ParsedPath`
- `relative(from: string, to: string): string`
- `resolve(paths: string[]): string`
- `toNamespacedPath(path: string): string`

**Properties:**

- `Instance: PathModule` (readonly)
- `delimiter: string` (readonly)
- `posix: PathModule` (readonly)
- `sep: string` (readonly)
- `win32: PathModule` (readonly)

### X509CertificateInfo
**Methods**: 7 | **Properties**: 10

**Methods:**

- `ToString(): string`
- `checkEmail(email: string): string`
- `checkHost(hostname: string): string`
- `checkIP(ip: string): string`
- `checkIssued(otherCert: X509CertificateInfo): string`
- `toPEM(): string`
- `verify(issuerCert: X509CertificateInfo): boolean`

**Properties:**

- `fingerprint: string` (readonly)
- `fingerprint256: string` (readonly)
- `fingerprint512: string` (readonly)
- `issuer: string` (readonly)
- `publicKey: Byte[]` (readonly)
- `raw: Byte[]` (readonly)
- `serialNumber: string` (readonly)
- `subject: string` (readonly)
- `validFrom: DateTime` (readonly)
- `validTo: DateTime` (readonly)

### ChildProcess
**Methods**: 5 | **Properties**: 11

**Methods:**

- `disconnect(): void`
- `kill(signal?: string): boolean`
- `ref(): void`
- `send(message: any, sendHandle?: any, options?: any, callback?: (arg0: Exception) => void): boolean`
- `unref(): void`

**Properties:**

- `connected: boolean` (get/set)
- `exitCode: number | null` (get/set)
- `killed: boolean` (readonly)
- `pid: number` (readonly)
- `referenced: boolean` (get/set)
- `signalCode: string` (get/set)
- `spawnargs: string[]` (get/set)
- `spawnfile: string` (get/set)
- `stderr: Readable` (get/set)
- `stdin: Writable` (get/set)
- `stdout: Readable` (get/set)

### URL
**Methods**: 4 | **Properties**: 12

**Methods:**

- `ToString(): string`
- `canParse(input: string, base?: string): boolean`
- `parse(input: string, base?: string): URL`
- `toJSON(): string`

**Properties:**

- `hash: string` (get/set)
- `host: string` (get/set)
- `hostname: string` (get/set)
- `href: string` (get/set)
- `origin: string` (readonly)
- `password: string` (get/set)
- `pathname: string` (get/set)
- `port: string` (get/set)
- `protocol: string` (get/set)
- `search: string` (get/set)
- `searchParams: URLSearchParams` (readonly)
- `username: string` (get/set)

### ExecOptions
**Methods**: 0 | **Properties**: 15

**Properties:**

- `argv0: string` (get/set)
- `cwd: string` (get/set)
- `detached: boolean` (get/set)
- `encoding: string` (get/set)
- `env: any` (get/set)
- `gid: number | null` (get/set)
- `input: string` (get/set)
- `killSignal: string` (get/set)
- `maxBuffer: number` (get/set)
- `shell: string` (get/set)
- `stdio: string` (get/set)
- `timeout: number` (get/set)
- `uid: number | null` (get/set)
- `windowsHide: boolean` (get/set)
- `windowsVerbatimArguments: boolean` (get/set)

### ProcessEnv
**Methods**: 10 | **Properties**: 5

**Methods:**

- `Add(key: string, value: string): void`
- `Add(item: KeyValuePair`2): void`
- `Clear(): void`
- `Contains(item: KeyValuePair`2): boolean`
- `ContainsKey(key: string): boolean`
- `CopyTo(array: KeyValuePair`2[], arrayIndex: number): void`
- `GetEnumerator(): IEnumerator`1`
- `Remove(key: string): boolean`
- `Remove(item: KeyValuePair`2): boolean`
- `TryGetValue(key: string, value: String&): boolean`

**Properties:**

- `Count: number` (readonly)
- `IsReadOnly: boolean` (readonly)
- `Item: string` (get/set)
- `Keys: ICollection`1` (readonly)
- `Values: ICollection`1` (readonly)

### Stats
**Methods**: 7 | **Properties**: 8

**Methods:**

- `IsBlockDevice(): boolean`
- `IsCharacterDevice(): boolean`
- `IsDirectory(): boolean`
- `IsFIFO(): boolean`
- `IsFile(): boolean`
- `IsSocket(): boolean`
- `IsSymbolicLink(): boolean`

**Properties:**

- `atime: DateTime` (get/set)
- `birthtime: DateTime` (get/set)
- `ctime: DateTime` (get/set)
- `isDirectory: boolean` (get/set)
- `isFile: boolean` (get/set)
- `mode: number` (get/set)
- `mtime: DateTime` (get/set)
- `size: Int64` (get/set)

### ECDH
**Methods**: 14 | **Properties**: 0

**Methods:**

- `Dispose(): void`
- `computeSecret(otherPublicKey: string, inputEncoding?: string, outputEncoding?: string): string`
- `computeSecret(otherPublicKey: Byte[], outputEncoding?: string): string`
- `computeSecret(otherPublicKey: Byte[]): Byte[]`
- `generateKeys(encoding?: string, format?: string): string`
- `generateKeys(): Byte[]`
- `getPrivateKey(encoding?: string): string`
- `getPrivateKey(): Byte[]`
- `getPublicKey(encoding?: string, format?: string): string`
- `getPublicKey(): Byte[]`
- `setPrivateKey(privateKey: string, encoding?: string): void`
- `setPrivateKey(privateKey: Byte[]): void`
- `setPublicKey(publicKey: string, encoding?: string): void`
- `setPublicKey(publicKey: Byte[]): void`

### Readable
**Methods**: 9 | **Properties**: 5

**Methods:**

- `destroy(error?: Exception): void`
- `isPaused(): boolean`
- `pause(): Readable`
- `push(chunk: any, encoding?: string): boolean`
- `read(size?: number | null): any`
- `resume(): Readable`
- `setEncoding(encoding: string): Readable`
- `unpipe(destination?: Stream): Readable`
- `unshift(chunk: any): void`

**Properties:**

- `destroyed: boolean` (get/set)
- `readable: boolean` (readonly)
- `readableEnded: boolean` (readonly)
- `readableFlowing: boolean | null` (readonly)
- `readableLength: number` (readonly)

### URLSearchParams
**Methods**: 12 | **Properties**: 1

**Methods:**

- `ToString(): string`
- `append(name: string, value: string): void`
- `delete(name: string, value?: string): void`
- `entries(): IEnumerable`1`
- `forEach(callback: Action`2): void`
- `get(name: string): string`
- `getAll(name: string): string[]`
- `has(name: string, value?: string): boolean`
- `keys(): IEnumerable`1`
- `set(name: string, value: string): void`
- `sort(): void`
- `values(): IEnumerable`1`

**Properties:**

- `size: number` (readonly)

### Interface
**Methods**: 10 | **Properties**: 2

**Methods:**

- `close(): void`
- `getCursorPos(): CursorPosition`
- `getPrompt(): string`
- `pause(): Interface`
- `prompt(preserveCursor?: boolean): void`
- `question(query: string, callback: (arg0: string) => void): void`
- `questionAsync(query: string): Task`1`
- `resume(): Interface`
- `setPrompt(prompt: string): void`
- `write(data: any, key?: any): void`

**Properties:**

- `cursor: number` (readonly)
- `line: string` (readonly)

### PeerCertificate
**Methods**: 0 | **Properties**: 12

**Properties:**

- `ca: boolean` (get/set)
- `ext_key_usage: string[]` (get/set)
- `fingerprint: string` (get/set)
- `fingerprint256: string` (get/set)
- `fingerprint512: string` (get/set)
- `issuer: TLSCertificateInfo` (get/set)
- `raw: Byte[]` (get/set)
- `serialNumber: string` (get/set)
- `subject: TLSCertificateInfo` (get/set)
- `subjectaltname: string` (get/set)
- `valid_from: string` (get/set)
- `valid_to: string` (get/set)

### Server
**Methods**: 10 | **Properties**: 2

**Methods:**

- `address(): any`
- `close(callback?: (arg0: Exception) => void): Server`
- `getConnections(callback: Action`2): void`
- `listen(port: number, hostname: string, backlog: number, listeningListener?: Action): Server`
- `listen(port: number, hostname: string, listeningListener?: Action): Server`
- `listen(port: number, backlog: number, listeningListener?: Action): Server`
- `listen(port: number, listeningListener?: Action): Server`
- `listen(options: ListenOptions, listeningListener?: Action): Server`
- `ref(): Server`
- `unref(): Server`

**Properties:**

- `listening: boolean` (readonly)
- `maxConnections: number` (get/set)

### Writable
**Methods**: 5 | **Properties**: 5

**Methods:**

- `cork(): void`
- `destroy(error?: Exception): void`
- `end(chunk?: any, encoding?: string, callback?: Action): void`
- `uncork(): void`
- `write(chunk: any, encoding?: string, callback?: Action): boolean`

**Properties:**

- `destroyed: boolean` (get/set)
- `writable: boolean` (readonly)
- `writableCorked: boolean` (readonly)
- `writableEnded: boolean` (readonly)
- `writableLength: number` (readonly)

### Duplex
**Methods**: 5 | **Properties**: 4

**Methods:**

- `cork(): void`
- `destroy(error?: Exception): void`
- `end(chunk?: any, encoding?: string, callback?: Action): void`
- `uncork(): void`
- `write(chunk: any, encoding?: string, callback?: Action): boolean`

**Properties:**

- `writable: boolean` (readonly)
- `writableCorked: boolean` (readonly)
- `writableEnded: boolean` (readonly)
- `writableLength: number` (readonly)

### InterfaceOptions
**Methods**: 0 | **Properties**: 9

**Properties:**

- `escapeCodeTimeout: number | null` (get/set)
- `history: string[]` (get/set)
- `historySize: number | null` (get/set)
- `input: Readable` (get/set)
- `output: Writable` (get/set)
- `prompt: string` (get/set)
- `removeHistoryDuplicates: boolean | null` (get/set)
- `tabSize: number | null` (get/set)
- `terminal: boolean | null` (get/set)

### TcpSocketConnectOpts
**Methods**: 0 | **Properties**: 9

**Properties:**

- `family: number | null` (get/set)
- `hints: number | null` (get/set)
- `host: string` (get/set)
- `keepAlive: boolean | null` (get/set)
- `keepAliveInitialDelay: number | null` (get/set)
- `localAddress: string` (get/set)
- `localPort: number | null` (get/set)
- `noDelay: boolean | null` (get/set)
- `port: number` (get/set)

### Cipher
**Methods**: 8 | **Properties**: 0

**Methods:**

- `Dispose(): void`
- `final(outputEncoding?: string): string`
- `final(): Byte[]`
- `getAuthTag(): Byte[]`
- `setAAD(buffer: Byte[]): void`
- `setAuthTag(tagLength: number): void`
- `update(data: string, inputEncoding?: string, outputEncoding?: string): string`
- `update(data: Byte[], outputEncoding?: string): string`

### ConnectionOptions
**Methods**: 0 | **Properties**: 8

**Properties:**

- `ca: any` (get/set)
- `cert: any` (get/set)
- `host: string` (get/set)
- `key: any` (get/set)
- `passphrase: string` (get/set)
- `port: number | null` (get/set)
- `servername: string` (get/set)
- `timeout: number | null` (get/set)

### SecureContextOptions
**Methods**: 0 | **Properties**: 8

**Properties:**

- `ca: any` (get/set)
- `cert: any` (get/set)
- `ciphers: string` (get/set)
- `key: any` (get/set)
- `maxVersion: string` (get/set)
- `minVersion: string` (get/set)
- `passphrase: string` (get/set)
- `pfx: any` (get/set)

### TlsOptions
**Methods**: 0 | **Properties**: 8

**Properties:**

- `allowHalfOpen: boolean | null` (get/set)
- `ca: any` (get/set)
- `cert: any` (get/set)
- `handshakeTimeout: number | null` (get/set)
- `key: any` (get/set)
- `passphrase: string` (get/set)
- `pauseOnConnect: boolean | null` (get/set)
- `sessionTimeout: number | null` (get/set)

### Decipher
**Methods**: 7 | **Properties**: 0

**Methods:**

- `Dispose(): void`
- `final(outputEncoding?: string): string`
- `final(): Byte[]`
- `setAAD(buffer: Byte[]): void`
- `setAuthTag(buffer: Byte[]): void`
- `update(data: string, inputEncoding?: string, outputEncoding?: string): string`
- `update(data: Byte[], outputEncoding?: string): string`

### Hash
**Methods**: 7 | **Properties**: 0

**Methods:**

- `Dispose(): void`
- `copy(): Hash`
- `digest(encoding: string): string`
- `digest(): Byte[]`
- `digest(outputLength: number): Byte[]`
- `update(data: string, inputEncoding?: string): Hash`
- `update(data: Byte[]): Hash`

### SecureContext
**Methods**: 3 | **Properties**: 4

**Methods:**

- `LoadCACertificates(ca: any): void`
- `LoadCertificate(cert: any, key: any, passphrase: string): void`
- `SetProtocols(minVersion: string, maxVersion: string): void`

**Properties:**

- `CACertificates: X509Certificate2Collection` (readonly)
- `Certificate: X509Certificate2` (readonly)
- `Protocols: SslProtocols` (readonly)
- `context: any` (get/set)

### Sign
**Methods**: 7 | **Properties**: 0

**Methods:**

- `Dispose(): void`
- `sign(privateKey: string, outputEncoding?: string): string`
- `sign(privateKey: string): Byte[]`
- `sign(privateKey: any, outputEncoding?: string): string`
- `sign(privateKey: any): Byte[]`
- `update(data: string, inputEncoding?: string): Sign`
- `update(data: Byte[]): Sign`

### SoaRecord
**Methods**: 0 | **Properties**: 7

**Properties:**

- `expire: number` (get/set)
- `hostmaster: string` (get/set)
- `minttl: number` (get/set)
- `nsname: string` (get/set)
- `refresh: number` (get/set)
- `retry: number` (get/set)
- `serial: number` (get/set)

### SpawnSyncReturns`1
**Methods**: 0 | **Properties**: 7

**Properties:**

- `error: Exception` (get/set)
- `output: T[]` (get/set)
- `pid: number` (get/set)
- `signal: string` (get/set)
- `status: number | null` (get/set)
- `stderr: T` (get/set)
- `stdout: T` (get/set)

### TLSSocketOptions
**Methods**: 0 | **Properties**: 7

**Properties:**

- `ca: any` (get/set)
- `cert: any` (get/set)
- `isServer: boolean | null` (get/set)
- `key: any` (get/set)
- `passphrase: string` (get/set)
- `server: Server` (get/set)
- `servername: string` (get/set)

### Verify
**Methods**: 7 | **Properties**: 0

**Methods:**

- `Dispose(): void`
- `update(data: string, inputEncoding?: string): Verify`
- `update(data: Byte[]): Verify`
- `verify(publicKey: string, signature: string, signatureEncoding?: string): boolean`
- `verify(publicKey: string, signature: Byte[]): boolean`
- `verify(publicKey: any, signature: string, signatureEncoding?: string): boolean`
- `verify(publicKey: any, signature: Byte[]): boolean`

### CaaRecord
**Methods**: 0 | **Properties**: 6

**Properties:**

- `contactemail: string` (get/set)
- `contactphone: string` (get/set)
- `critical: number` (get/set)
- `iodef: string` (get/set)
- `issue: string` (get/set)
- `issuewild: string` (get/set)

### NaptrRecord
**Methods**: 0 | **Properties**: 6

**Properties:**

- `flags: string` (get/set)
- `order: number` (get/set)
- `preference: number` (get/set)
- `regexp: string` (get/set)
- `replacement: string` (get/set)
- `service: string` (get/set)

### PrivateKeyObject
**Methods**: 3 | **Properties**: 3

**Methods:**

- `Dispose(): void`
- `export(options?: any): any`
- `export(format: string, type?: string, cipher?: string, passphrase?: string): string`

**Properties:**

- `asymmetricKeyType: string` (readonly)
- `symmetricKeySize: number | null` (readonly)
- `type: string` (readonly)

### PublicKeyObject
**Methods**: 3 | **Properties**: 3

**Methods:**

- `Dispose(): void`
- `export(options?: any): any`
- `export(format: string, type?: string): string`

**Properties:**

- `asymmetricKeyType: string` (readonly)
- `symmetricKeySize: number | null` (readonly)
- `type: string` (readonly)


---

## PART 3: CORE NODE.JS MODULES - DETAILED ANALYSIS


### path
**Type**: Static Module
**API Count**: 12 methods, 2 properties

**Complete API:**

- `basename(path: string, suffix?: string): string`
- `dirname(path: string): string`
- `extname(path: string): string`
- `format(pathObject: ParsedPath): string`
- `isAbsolute(path: string): boolean`
- `join(paths: string[]): string`
- `matchesGlob(path: string, pattern: string): boolean`
- `normalize(path: string): string`
- `parse(path: string): ParsedPath`
- `relative(from: string, to: string): string`
- `resolve(paths: string[]): string`
- `toNamespacedPath(path: string): string`

**Properties:**

- `posix: PathModule` (readonly)
- `win32: PathModule` (readonly)

### fs
**Type**: Static Module
**API Count**: 53 methods, 0 properties

**Complete API:**

- `access(path: string, mode?: number = 0): Task`
- `accessSync(path: string, mode?: number = 0): void`
- `appendFile(path: string, data: string, encoding?: string = utf-8): Task`
- `appendFileSync(path: string, data: string, encoding?: string = utf-8): void`
- `chmod(path: string, mode: number): Task`
- `chmodSync(path: string, mode: number): void`
- `close(fd: number): Task`
- `closeSync(fd: number): void`
- `copyFile(src: string, dest: string, mode?: number = 0): Task`
- `copyFileSync(src: string, dest: string, mode?: number = 0): void`
- `cp(src: string, dest: string, recursive?: boolean = False): Task`
- `cpSync(src: string, dest: string, recursive?: boolean = False): void`
- `existsSync(path: string): boolean`
- `fstat(fd: number): Task`1`
- `fstatSync(fd: number): Stats`
- `mkdir(path: string, recursive?: boolean = False): Task`
- `mkdirSync(path: string, recursive?: boolean = False): void`
- `open(path: string, flags: string, mode?: number | null): Task`1`
- `openSync(path: string, flags: string, mode?: number | null): number`
- `read(fd: number, buffer: Byte[], offset: number, length: number, position: number | null): Task`1`
- `readFile(path: string, encoding?: string = utf-8): Task`1`
- `readFileBytes(path: string): Task`1`
- `readFileSync(path: string, encoding?: string = utf-8): string`
- `readFileSyncBytes(path: string): Byte[]`
- `readSync(fd: number, buffer: Byte[], offset: number, length: number, position: number | null): number`
- `readdir(path: string, withFileTypes?: boolean = False): Task`1`
- `readdirSync(path: string, withFileTypes?: boolean = False): string[]`
- `readlink(path: string): Task`1`
- `readlinkSync(path: string): string`
- `realpath(path: string): Task`1`
- `realpathSync(path: string): string`
- `rename(oldPath: string, newPath: string): Task`
- `renameSync(oldPath: string, newPath: string): void`
- `rm(path: string, recursive?: boolean = False): Task`
- `rmSync(path: string, recursive?: boolean = False): void`
- `rmdir(path: string, recursive?: boolean = False): Task`
- `rmdirSync(path: string, recursive?: boolean = False): void`
- `stat(path: string): Task`1`
- `statSync(path: string): Stats`
- `symlink(target: string, path: string, type?: string): Task`
- `symlinkSync(target: string, path: string, type?: string): void`
- `truncate(path: string, len?: Int64 = 0): Task`
- `truncateSync(path: string, len?: Int64 = 0): void`
- `unlink(path: string): Task`
- `unlinkSync(path: string): void`
- `write(fd: number, buffer: Byte[], offset: number, length: number, position: number | null): Task`1`
- `write(fd: number, data: string, position?: number | null, encoding?: string): Task`1`
- `writeFile(path: string, data: string, encoding?: string = utf-8): Task`
- `writeFileBytes(path: string, data: Byte[]): Task`
- `writeFileSync(path: string, data: string, encoding?: string = utf-8): void`
- `writeFileSyncBytes(path: string, data: Byte[]): void`
- `writeSync(fd: number, buffer: Byte[], offset: number, length: number, position: number | null): number`
- `writeSync(fd: number, data: string, position?: number | null, encoding?: string): number`

### crypto
**Type**: Static Module
**API Count**: 63 methods, 0 properties

**Complete API:**

- `createCipheriv(algorithm: string, key: Byte[], iv: Byte[]): Cipher`
- `createCipheriv(algorithm: string, key: string, iv: string): Cipher`
- `createDecipheriv(algorithm: string, key: Byte[], iv: Byte[]): Decipher`
- `createDecipheriv(algorithm: string, key: string, iv: string): Decipher`
- `createDiffieHellman(primeLength: number, generator?: number = 2): DiffieHellman`
- `createDiffieHellman(prime: Byte[], generator: Byte[]): DiffieHellman`
- `createDiffieHellman(prime: Byte[], generator?: number = 2): DiffieHellman`
- `createDiffieHellman(prime: string, primeEncoding: string, generator?: number = 2): DiffieHellman`
- `createDiffieHellman(prime: string, primeEncoding: string, generator: string, generatorEncoding: string): DiffieHellman`
- `createECDH(curveName: string): ECDH`
- `createHash(algorithm: string): Hash`
- `createHmac(algorithm: string, key: string): Hmac`
- `createHmac(algorithm: string, key: Byte[]): Hmac`
- `createPrivateKey(key: string): KeyObject`
- `createPrivateKey(key: Byte[]): KeyObject`
- `createPublicKey(key: string): KeyObject`
- `createPublicKey(key: Byte[]): KeyObject`
- `createPublicKey(key: KeyObject): KeyObject`
- `createSecretKey(key: Byte[]): KeyObject`
- `createSecretKey(key: string, encoding?: string): KeyObject`
- `createSign(algorithm: string): Sign`
- `createVerify(algorithm: string): Verify`
- `generateKey(type: string, options: any): KeyObject`
- `generateKey(type: string, options: any, callback: Action`2): void`
- `generateKeyPair(type: string, options: any, callback: Action`3): void`
- `generateKeyPairSync(type: string, options?: any): ValueTuple`2`
- `getCiphers(): string[]`
- `getCurves(): string[]`
- `getDefaultCipherList(): string`
- `getDiffieHellman(groupName: string): DiffieHellman`
- `getFips(): boolean`
- `getHashes(): string[]`
- `hash(algorithm: string, data: Byte[], outputEncoding?: string): Byte[]`
- `hkdf(digest: string, ikm: Byte[], salt: Byte[], info: Byte[], keylen: number, callback: Action`2): void`
- `hkdfSync(digest: string, ikm: Byte[], salt: Byte[], info: Byte[], keylen: number): Byte[]`
- `pbkdf2(password: string, salt: string, iterations: number, keylen: number, digest: string, callback: Action`2): void`
- `pbkdf2Sync(password: string, salt: string, iterations: number, keylen: number, digest: string): Byte[]`
- `pbkdf2Sync(password: Byte[], salt: Byte[], iterations: number, keylen: number, digest: string): Byte[]`
- `privateDecrypt(key: string, buffer: Byte[]): Byte[]`
- `privateDecrypt(key: any, buffer: Byte[]): Byte[]`
- `privateEncrypt(key: string, buffer: Byte[]): Byte[]`
- `privateEncrypt(key: any, buffer: Byte[]): Byte[]`
- `publicDecrypt(key: string, buffer: Byte[]): Byte[]`
- `publicDecrypt(key: any, buffer: Byte[]): Byte[]`
- `publicEncrypt(key: string, buffer: Byte[]): Byte[]`
- `publicEncrypt(key: any, buffer: Byte[]): Byte[]`
- `randomBytes(size: number): Byte[]`
- `randomBytes(size: number, callback: Action`2): void`
- `randomFill(buffer: Byte[], offset: number, size: number, callback: Action`2): void`
- `randomFillSync(buffer: Byte[], offset?: number = 0, size?: number | null): Byte[]`
- `randomInt(max: number): number`
- `randomInt(min: number, max: number): number`
- `randomUUID(): string`
- `scrypt(password: string, salt: string, keylen: number, options: any, callback: Action`2): void`
- `scryptSync(password: string, salt: string, keylen: number, options?: any): Byte[]`
- `scryptSync(password: Byte[], salt: Byte[], keylen: number, options?: any): Byte[]`
- `setDefaultEncoding(encoding: string): void`
- `setFips(enabled: boolean): void`
- `sign(algorithm: string, data: Byte[], privateKey: string): Byte[]`
- `sign(algorithm: string, data: Byte[], privateKey: KeyObject): Byte[]`
- `timingSafeEqual(a: Byte[], b: Byte[]): boolean`
- `verify(algorithm: string, data: Byte[], publicKey: string, signature: Byte[]): boolean`
- `verify(algorithm: string, data: Byte[], publicKey: KeyObject, signature: Byte[]): boolean`

### buffer - ❌ NOT FOUND

### events - ❌ NOT FOUND

### os
**Type**: Static Module
**API Count**: 15 methods, 0 properties

**Complete API:**

- `arch(): string`
- `availableParallelism(): number`
- `cpus(): CpuInfo[]`
- `endianness(): string`
- `freemem(): Int64`
- `homedir(): string`
- `hostname(): string`
- `loadavg(): number[]`
- `platform(): string`
- `release(): string`
- `tmpdir(): string`
- `totalmem(): Int64`
- `type(): string`
- `uptime(): Int64`
- `userInfo(): UserInfo`

### util
**Type**: Static Module
**API Count**: 8 methods, 0 properties

**Complete API:**

- `debuglog(section: string): DebugLogFunction`
- `deprecate(fn: () => TResult, msg: string, code?: string): () => TResult`
- `deprecate(action: Action, msg: string, code?: string): Action`
- `format(format: any, args: any[]): string`
- `inherits(constructor: any, superConstructor: any): void`
- `inspect(obj: any): string`
- `isArray(obj: any): boolean`
- `isDeepStrictEqual(val1: any, val2: any): boolean`

### child_process
**Type**: Static Module
**API Count**: 10 methods, 0 properties

**Complete API:**

- `exec(command: string, options: ExecOptions, callback: Action`3): void`
- `exec(command: string, callback: Action`3): void`
- `execFile(file: string, args: string[], options: ExecOptions, callback: Action`3): void`
- `execFileSync(file: string, args?: string[], options?: ExecOptions): any`
- `execSync(command: string): Byte[]`
- `execSync(command: string, options: ExecOptions): any`
- `fork(modulePath: string, args?: string[], options?: ExecOptions): ChildProcess`
- `spawn(command: string, args?: string[], options?: ExecOptions): ChildProcess`
- `spawnSync(command: string, args?: string[], options?: ExecOptions): SpawnSyncReturns`1`
- `spawnSyncString(command: string, args?: string[], options?: ExecOptions): SpawnSyncReturns`1`

### stream
**Type**: Static Module
**API Count**: 3 methods, 0 properties

**Complete API:**

- `finished(stream: Stream, callback: (arg0: Exception) => void): void`
- `finished(stream: Stream): Task`
- `pipeline(streams: any[]): void`


---

## PART 4: STATISTICS


### Module Counts
- Static Modules: 21
- Instance Classes: 107
- Data Types: 0
- **Total: 128**

### Method Counts (Top 10 Static Modules)
- crypto: 63 methods
- fs: 53 methods
- dns: 27 methods
- console: 22 methods
- assert: 15 methods
- net: 15 methods
- os: 15 methods
- path: 12 methods
- zlib: 11 methods
- child_process: 10 methods

### Method Counts (Top 10 Instance Classes)
- Buffer: 91 methods, 3 properties
- DgramSocket: 36 methods, 0 properties
- Resolver: 22 methods, 0 properties
- TLSSocket: 22 methods, 4 properties
- Socket: 20 methods, 11 properties
- DiffieHellman: 19 methods, 0 properties
- EventEmitter: 16 methods, 1 properties
- ECDH: 14 methods, 0 properties
- PathModule: 12 methods, 5 properties
- URLSearchParams: 12 methods, 1 properties


---

## PART 5: POTENTIALLY CUSTOM APIs (Not in Standard Node.js)


These methods were found in Tsonic.Node but may not exist in standard Node.js:

- **crypto.getDefaultCipherList(): string**
  → Custom addition or requires verification
- **crypto.setDefaultEncoding(encoding: string): void**
  → Custom addition or requires verification
- **fs.readFileBytes(path: string): Task`1**
  → Custom addition or requires verification
- **fs.readFileSyncBytes(path: string): Byte[]**
  → Custom addition or requires verification
- **fs.writeFileBytes(path: string, data: Byte[]): Task**
  → Custom addition or requires verification
- **fs.writeFileSyncBytes(path: string, data: Byte[]): void**
  → Custom addition or requires verification
- **path.matchesGlob(path: string, pattern: string): boolean**
  → Custom addition or requires verification