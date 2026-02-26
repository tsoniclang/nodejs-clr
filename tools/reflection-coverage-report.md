# Reflection Coverage Report (Node runtime vs nodejs-clr reflection)

Generated: 2026-02-26T02:28:15.810Z
Node runtime snapshot: 2026-02-26T02:28:15.506Z
Node version: v24.7.0

## Scope

- Node side: runtime reflection of builtin modules.
- CLR side: reflection over public static module types in the `nodejs` assembly.
- Comparison is name-level API coverage (not overload/type compatibility).

## Coverage Table

| Module | Covered / Node exports | Coverage |
|---|---:|---:|
| `async_hooks` | 0 / 7 | 0.0% |
| `cluster` | 0 / 16 | 0.0% |
| `constants` | 0 / 235 | 0.0% |
| `diagnostics_channel` | 0 / 6 | 0.0% |
| `domain` | 0 / 5 | 0.0% |
| `fs/promises` | 0 / 33 | 0.0% |
| `http2` | 0 / 11 | 0.0% |
| `inspector` | 0 / 8 | 0.0% |
| `module` | 0 / 34 | 0.0% |
| `path/posix` | 0 / 17 | 0.0% |
| `path/win32` | 0 / 17 | 0.0% |
| `punycode` | 0 / 6 | 0.0% |
| `repl` | 0 / 8 | 0.0% |
| `sea` | 0 / 4 | 0.0% |
| `sqlite` | 0 / 4 | 0.0% |
| `stream/consumers` | 0 / 5 | 0.0% |
| `stream/promises` | 0 / 2 | 0.0% |
| `stream/web` | 0 / 17 | 0.0% |
| `test` | 0 / 15 | 0.0% |
| `timers/promises` | 0 / 4 | 0.0% |
| `trace_events` | 0 / 2 | 0.0% |
| `tty` | 0 / 3 | 0.0% |
| `util/types` | 0 / 43 | 0.0% |
| `v8` | 0 / 22 | 0.0% |
| `vm` | 0 / 10 | 0.0% |
| `wasi` | 0 / 1 | 0.0% |
| `worker_threads` | 0 / 21 | 0.0% |
| `dns/promises` | 1 / 46 | 2.2% |
| `assert/strict` | 1 / 22 | 4.5% |
| `zlib` | 10 / 154 | 6.5% |
| `https` | 1 / 6 | 16.7% |
| `process` | 15 / 86 | 17.4% |
| `util` | 10 / 34 | 29.4% |
| `stream` | 7 / 23 | 30.4% |
| `readline/promises` | 1 / 3 | 33.3% |
| `fs` | 49 / 108 | 45.4% |
| `perf_hooks` | 5 / 11 | 45.5% |
| `dns` | 23 / 50 | 46.0% |
| `http` | 10 / 20 | 50.0% |
| `tls` | 9 / 18 | 50.0% |
| `crypto` | 47 / 73 | 64.4% |
| `dgram` | 2 / 3 | 66.7% |
| `events` | 12 / 17 | 70.6% |
| `os` | 17 / 23 | 73.9% |
| `console` | 23 / 31 | 74.2% |
| `net` | 13 / 17 | 76.5% |
| `path` | 14 / 17 | 82.4% |
| `buffer` | 12 / 14 | 85.7% |
| `querystring` | 6 / 7 | 85.7% |
| `url` | 12 / 14 | 85.7% |
| `assert` | 19 / 22 | 86.4% |
| `child_process` | 8 / 9 | 88.9% |
| `readline` | 8 / 8 | 100.0% |
| `string_decoder` | 1 / 1 | 100.0% |
| `timers` | 7 / 7 | 100.0% |

## Per-module Gaps

### async_hooks

- Coverage: **0 / 7** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (7)

- `AsyncLocalStorage`, `AsyncResource`, `asyncWrapProviders`, `createHook`, `executionAsyncId`, `executionAsyncResource`, `triggerAsyncId`

### cluster

- Coverage: **0 / 16** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (16)

- `_events`, `_eventsCount`, `_maxListeners`, `disconnect`, `fork`, `isMaster`, `isPrimary`, `isWorker`, `SCHED_NONE`, `SCHED_RR`, `schedulingPolicy`, `settings`, `setupMaster`, `setupPrimary`, `Worker`, `workers`

### constants

- Coverage: **0 / 235** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (235)

- `COPYFILE_EXCL`, `COPYFILE_FICLONE`, `COPYFILE_FICLONE_FORCE`, `defaultCipherList`, `defaultCoreCipherList`, `DH_CHECK_P_NOT_PRIME`, `DH_CHECK_P_NOT_SAFE_PRIME`, `DH_NOT_SUITABLE_GENERATOR`, `DH_UNABLE_TO_CHECK_GENERATOR`, `E2BIG`, `EACCES`, `EADDRINUSE`, `EADDRNOTAVAIL`, `EAFNOSUPPORT`, `EAGAIN`, `EALREADY`, `EBADF`, `EBADMSG`, `EBUSY`, `ECANCELED`, `ECHILD`, `ECONNABORTED`, `ECONNREFUSED`, `ECONNRESET`, `EDEADLK`, `EDESTADDRREQ`, `EDOM`, `EDQUOT`, `EEXIST`, `EFAULT`, `EFBIG`, `EHOSTUNREACH`, `EIDRM`, `EILSEQ`, `EINPROGRESS`, `EINTR`, `EINVAL`, `EIO`, `EISCONN`, `EISDIR`, `ELOOP`, `EMFILE`, `EMLINK`, `EMSGSIZE`, `EMULTIHOP`, `ENAMETOOLONG`, `ENETDOWN`, `ENETRESET`, `ENETUNREACH`, `ENFILE`, `ENGINE_METHOD_ALL`, `ENGINE_METHOD_CIPHERS`, `ENGINE_METHOD_DH`, `ENGINE_METHOD_DIGESTS`, `ENGINE_METHOD_DSA`, `ENGINE_METHOD_EC`, `ENGINE_METHOD_NONE`, `ENGINE_METHOD_PKEY_ASN1_METHS`, `ENGINE_METHOD_PKEY_METHS`, `ENGINE_METHOD_RAND`, `ENGINE_METHOD_RSA`, `ENOBUFS`, `ENODATA`, `ENODEV`, `ENOENT`, `ENOEXEC`, `ENOLCK`, `ENOLINK`, `ENOMEM`, `ENOMSG`, `ENOPROTOOPT`, `ENOSPC`, `ENOSR`, `ENOSTR`, `ENOSYS`, `ENOTCONN`, `ENOTDIR`, `ENOTEMPTY`, `ENOTSOCK`, `ENOTSUP`, `ENOTTY`, `ENXIO`, `EOPNOTSUPP`, `EOVERFLOW`, `EPERM`, `EPIPE`, `EPROTO`, `EPROTONOSUPPORT`, `EPROTOTYPE`, `ERANGE`, `EROFS`, `ESPIPE`, `ESRCH`, `ESTALE`, `ETIME`, `ETIMEDOUT`, `ETXTBSY`, `EWOULDBLOCK`, `EXDEV`, `F_OK`, `O_APPEND`, `O_CREAT`, `O_DIRECT`, `O_DIRECTORY`, `O_DSYNC`, `O_EXCL`, `O_NOATIME`, `O_NOCTTY`, `O_NOFOLLOW`, `O_NONBLOCK`, `O_RDONLY`, `O_RDWR`, `O_SYNC`, `O_TRUNC`, `O_WRONLY`, `OPENSSL_VERSION_NUMBER`, `POINT_CONVERSION_COMPRESSED`, `POINT_CONVERSION_HYBRID`, `POINT_CONVERSION_UNCOMPRESSED`, `PRIORITY_ABOVE_NORMAL`, `PRIORITY_BELOW_NORMAL`, `PRIORITY_HIGH`, `PRIORITY_HIGHEST`, `PRIORITY_LOW`, `PRIORITY_NORMAL`, `R_OK`, `RSA_NO_PADDING`, `RSA_PKCS1_OAEP_PADDING`, `RSA_PKCS1_PADDING`, `RSA_PKCS1_PSS_PADDING`, `RSA_PSS_SALTLEN_AUTO`, `RSA_PSS_SALTLEN_DIGEST`, `RSA_PSS_SALTLEN_MAX_SIGN`, `RSA_X931_PADDING`, `RTLD_DEEPBIND`, `RTLD_GLOBAL`, `RTLD_LAZY`, `RTLD_LOCAL`, `RTLD_NOW`, `S_IFBLK`, `S_IFCHR`, `S_IFDIR`, `S_IFIFO`, `S_IFLNK`, `S_IFMT`, `S_IFREG`, `S_IFSOCK`, `S_IRGRP`, `S_IROTH`, `S_IRUSR`, `S_IRWXG`, `S_IRWXO`, `S_IRWXU`, `S_IWGRP`, `S_IWOTH`, `S_IWUSR`, `S_IXGRP`, `S_IXOTH`, `S_IXUSR`, `SIGABRT`, `SIGALRM`, `SIGBUS`, `SIGCHLD`, `SIGCONT`, `SIGFPE`, `SIGHUP`, `SIGILL`, `SIGINT`, `SIGIO`, `SIGIOT`, `SIGKILL`, `SIGPIPE`, `SIGPOLL`, `SIGPROF`, `SIGPWR`, `SIGQUIT`, `SIGSEGV`, `SIGSTKFLT`, `SIGSTOP`, `SIGSYS`, `SIGTERM`, `SIGTRAP`, `SIGTSTP`, `SIGTTIN`, `SIGTTOU`, `SIGURG`, `SIGUSR1`, `SIGUSR2`, `SIGVTALRM`, `SIGWINCH`, `SIGXCPU`, `SIGXFSZ`, `SSL_OP_ALL`, `SSL_OP_ALLOW_NO_DHE_KEX`, `SSL_OP_ALLOW_UNSAFE_LEGACY_RENEGOTIATION`, `SSL_OP_CIPHER_SERVER_PREFERENCE`, `SSL_OP_CISCO_ANYCONNECT`, `SSL_OP_COOKIE_EXCHANGE`, `SSL_OP_CRYPTOPRO_TLSEXT_BUG`, `SSL_OP_DONT_INSERT_EMPTY_FRAGMENTS`, `SSL_OP_LEGACY_SERVER_CONNECT`, `SSL_OP_NO_COMPRESSION`, `SSL_OP_NO_ENCRYPT_THEN_MAC`, `SSL_OP_NO_QUERY_MTU`, `SSL_OP_NO_RENEGOTIATION`, `SSL_OP_NO_SESSION_RESUMPTION_ON_RENEGOTIATION`, `SSL_OP_NO_SSLv2`, `SSL_OP_NO_SSLv3`, `SSL_OP_NO_TICKET`, `SSL_OP_NO_TLSv1`, `SSL_OP_NO_TLSv1_1`, `SSL_OP_NO_TLSv1_2`, `SSL_OP_NO_TLSv1_3`, `SSL_OP_PRIORITIZE_CHACHA`, `SSL_OP_TLS_ROLLBACK_BUG`, `TLS1_1_VERSION`, `TLS1_2_VERSION`, `TLS1_3_VERSION`, `TLS1_VERSION`, `UV_DIRENT_BLOCK`, `UV_DIRENT_CHAR`, `UV_DIRENT_DIR`, `UV_DIRENT_FIFO`, `UV_DIRENT_FILE`, `UV_DIRENT_LINK`, `UV_DIRENT_SOCKET`, `UV_DIRENT_UNKNOWN`, `UV_FS_COPYFILE_EXCL`, `UV_FS_COPYFILE_FICLONE`, `UV_FS_COPYFILE_FICLONE_FORCE`, `UV_FS_O_FILEMAP`, `UV_FS_SYMLINK_DIR`, `UV_FS_SYMLINK_JUNCTION`, `W_OK`, `X_OK`

### diagnostics_channel

- Coverage: **0 / 6** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (6)

- `channel`, `Channel`, `hasSubscribers`, `subscribe`, `tracingChannel`, `unsubscribe`

### domain

- Coverage: **0 / 5** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (5)

- `_stack`, `active`, `create`, `createDomain`, `Domain`

### fs/promises

- Coverage: **0 / 33** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (33)

- `access`, `appendFile`, `chmod`, `chown`, `constants`, `copyFile`, `cp`, `glob`, `lchmod`, `lchown`, `link`, `lstat`, `lutimes`, `mkdir`, `mkdtemp`, `mkdtempDisposable`, `open`, `opendir`, `readdir`, `readFile`, `readlink`, `realpath`, `rename`, `rm`, `rmdir`, `stat`, `statfs`, `symlink`, `truncate`, `unlink`, `utimes`, `watch`, `writeFile`

### http2

- Coverage: **0 / 11** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (11)

- `connect`, `constants`, `createSecureServer`, `createServer`, `getDefaultSettings`, `getPackedSettings`, `getUnpackedSettings`, `Http2ServerRequest`, `Http2ServerResponse`, `performServerHandshake`, `sensitiveHeaders`

### inspector

- Coverage: **0 / 8** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (8)

- `close`, `console`, `Network`, `NetworkResources`, `open`, `Session`, `url`, `waitForDebugger`

### module

- Coverage: **0 / 34** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (34)

- `_cache`, `_debug`, `_extensions`, `_findPath`, `_initPaths`, `_load`, `_nodeModulePaths`, `_pathCache`, `_preloadModules`, `_readPackage`, `_resolveFilename`, `_resolveLookupPaths`, `_stat`, `builtinModules`, `constants`, `createRequire`, `enableCompileCache`, `findPackageJSON`, `findSourceMap`, `flushCompileCache`, `getCompileCacheDir`, `getSourceMapsSupport`, `globalPaths`, `isBuiltin`, `Module`, `register`, `registerHooks`, `runMain`, `setSourceMapsSupport`, `SourceMap`, `stripTypeScriptTypes`, `syncBuiltinESMExports`, `wrap`, `wrapper`

### path/posix

- Coverage: **0 / 17** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (17)

- `_makeLong`, `basename`, `delimiter`, `dirname`, `extname`, `format`, `isAbsolute`, `join`, `matchesGlob`, `normalize`, `parse`, `posix`, `relative`, `resolve`, `sep`, `toNamespacedPath`, `win32`

### path/win32

- Coverage: **0 / 17** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (17)

- `_makeLong`, `basename`, `delimiter`, `dirname`, `extname`, `format`, `isAbsolute`, `join`, `matchesGlob`, `normalize`, `parse`, `posix`, `relative`, `resolve`, `sep`, `toNamespacedPath`, `win32`

### punycode

- Coverage: **0 / 6** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (6)

- `decode`, `encode`, `toASCII`, `toUnicode`, `ucs2`, `version`

### repl

- Coverage: **0 / 8** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (8)

- `_builtinLibs`, `builtinModules`, `Recoverable`, `REPL_MODE_SLOPPY`, `REPL_MODE_STRICT`, `REPLServer`, `start`, `writer`

### sea

- Coverage: **0 / 4** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (4)

- `getAsset`, `getAssetAsBlob`, `getRawAsset`, `isSea`

### sqlite

- Coverage: **0 / 4** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (4)

- `backup`, `constants`, `DatabaseSync`, `StatementSync`

### stream/consumers

- Coverage: **0 / 5** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (5)

- `arrayBuffer`, `blob`, `buffer`, `json`, `text`

### stream/promises

- Coverage: **0 / 2** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (2)

- `finished`, `pipeline`

### stream/web

- Coverage: **0 / 17** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (17)

- `ByteLengthQueuingStrategy`, `CompressionStream`, `CountQueuingStrategy`, `DecompressionStream`, `ReadableByteStreamController`, `ReadableStream`, `ReadableStreamBYOBReader`, `ReadableStreamBYOBRequest`, `ReadableStreamDefaultController`, `ReadableStreamDefaultReader`, `TextDecoderStream`, `TextEncoderStream`, `TransformStream`, `TransformStreamDefaultController`, `WritableStream`, `WritableStreamDefaultController`, `WritableStreamDefaultWriter`

### test

- Coverage: **0 / 15** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (15)

- `after`, `afterEach`, `assert`, `before`, `beforeEach`, `describe`, `it`, `mock`, `only`, `run`, `skip`, `snapshot`, `suite`, `test`, `todo`

### timers/promises

- Coverage: **0 / 4** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (4)

- `scheduler`, `setImmediate`, `setInterval`, `setTimeout`

### trace_events

- Coverage: **0 / 2** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (2)

- `createTracing`, `getEnabledCategories`

### tty

- Coverage: **0 / 3** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (3)

- `isatty`, `ReadStream`, `WriteStream`

### util/types

- Coverage: **0 / 43** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (43)

- `isAnyArrayBuffer`, `isArgumentsObject`, `isArrayBuffer`, `isArrayBufferView`, `isAsyncFunction`, `isBigInt64Array`, `isBigIntObject`, `isBigUint64Array`, `isBooleanObject`, `isBoxedPrimitive`, `isCryptoKey`, `isDataView`, `isDate`, `isExternal`, `isFloat16Array`, `isFloat32Array`, `isFloat64Array`, `isGeneratorFunction`, `isGeneratorObject`, `isInt16Array`, `isInt32Array`, `isInt8Array`, `isKeyObject`, `isMap`, `isMapIterator`, `isModuleNamespaceObject`, `isNativeError`, `isNumberObject`, `isPromise`, `isProxy`, `isRegExp`, `isSet`, `isSetIterator`, `isSharedArrayBuffer`, `isStringObject`, `isSymbolObject`, `isTypedArray`, `isUint16Array`, `isUint32Array`, `isUint8Array`, `isUint8ClampedArray`, `isWeakMap`, `isWeakSet`

### v8

- Coverage: **0 / 22** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (22)

- `cachedDataVersionTag`, `DefaultDeserializer`, `DefaultSerializer`, `deserialize`, `Deserializer`, `GCProfiler`, `getCppHeapStatistics`, `getHeapCodeStatistics`, `getHeapSnapshot`, `getHeapSpaceStatistics`, `getHeapStatistics`, `isStringOneByteRepresentation`, `promiseHooks`, `queryObjects`, `serialize`, `Serializer`, `setFlagsFromString`, `setHeapSnapshotNearHeapLimit`, `startupSnapshot`, `stopCoverage`, `takeCoverage`, `writeHeapSnapshot`

### vm

- Coverage: **0 / 10** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (10)

- `compileFunction`, `constants`, `createContext`, `createScript`, `isContext`, `measureMemory`, `runInContext`, `runInNewContext`, `runInThisContext`, `Script`

### wasi

- Coverage: **0 / 1** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (1)

- `WASI`

### worker_threads

- Coverage: **0 / 21** (0.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (21)

- `BroadcastChannel`, `getEnvironmentData`, `isInternalThread`, `isMainThread`, `isMarkedAsUntransferable`, `locks`, `markAsUncloneable`, `markAsUntransferable`, `MessageChannel`, `MessagePort`, `moveMessagePortToContext`, `parentPort`, `postMessageToThread`, `receiveMessageOnPort`, `resourceLimits`, `setEnvironmentData`, `SHARE_ENV`, `threadId`, `threadName`, `Worker`, `workerData`

### dns/promises

- Coverage: **1 / 46** (2.2%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (45)

- `ADDRGETNETWORKPARAMS`, `BADFAMILY`, `BADFLAGS`, `BADHINTS`, `BADNAME`, `BADQUERY`, `BADRESP`, `BADSTR`, `CANCELLED`, `CONNREFUSED`, `DESTRUCTION`, `EOF`, `FILE`, `FORMERR`, `getDefaultResultOrder`, `getServers`, `LOADIPHLPAPI`, `lookup`, `lookupService`, `NODATA`, `NOMEM`, `NONAME`, `NOTFOUND`, `NOTIMP`, `NOTINITIALIZED`, `REFUSED`, `resolve`, `resolve4`, `resolve6`, `resolveAny`, `resolveCaa`, `resolveCname`, `resolveMx`, `resolveNaptr`, `resolveNs`, `resolvePtr`, `resolveSoa`, `resolveSrv`, `resolveTlsa`, `resolveTxt`, `reverse`, `SERVFAIL`, `setDefaultResultOrder`, `setServers`, `TIMEOUT`

### assert/strict

- Coverage: **1 / 22** (4.5%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (21)

- `Assert`, `CallTracker`, `deepEqual`, `deepStrictEqual`, `doesNotMatch`, `doesNotReject`, `doesNotThrow`, `equal`, `fail`, `ifError`, `match`, `notDeepEqual`, `notDeepStrictEqual`, `notEqual`, `notStrictEqual`, `ok`, `partialDeepStrictEqual`, `rejects`, `strict`, `strictEqual`, `throws`

### zlib

- Coverage: **10 / 154** (6.5%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (144)

- `brotliCompress`, `BrotliCompress`, `brotliDecompress`, `BrotliDecompress`, `codes`, `constants`, `createBrotliCompress`, `createBrotliDecompress`, `createDeflate`, `createDeflateRaw`, `createGunzip`, `createGzip`, `createInflate`, `createInflateRaw`, `createUnzip`, `createZstdCompress`, `createZstdDecompress`, `deflate`, `Deflate`, `DEFLATE`, `deflateRaw`, `DeflateRaw`, `DEFLATERAW`, `gunzip`, `Gunzip`, `GUNZIP`, `gzip`, `Gzip`, `GZIP`, `inflate`, `Inflate`, `INFLATE`, `inflateRaw`, `InflateRaw`, `INFLATERAW`, `unzip`, `Unzip`, `UNZIP`, `Z_BEST_COMPRESSION`, `Z_BEST_SPEED`, `Z_BLOCK`, `Z_BUF_ERROR`, `Z_DATA_ERROR`, `Z_DEFAULT_CHUNK`, `Z_DEFAULT_COMPRESSION`, `Z_DEFAULT_LEVEL`, `Z_DEFAULT_MEMLEVEL`, `Z_DEFAULT_STRATEGY`, `Z_DEFAULT_WINDOWBITS`, `Z_ERRNO`, `Z_FILTERED`, `Z_FINISH`, `Z_FIXED`, `Z_FULL_FLUSH`, `Z_HUFFMAN_ONLY`, `Z_MAX_CHUNK`, `Z_MAX_LEVEL`, `Z_MAX_MEMLEVEL`, `Z_MAX_WINDOWBITS`, `Z_MEM_ERROR`, `Z_MIN_CHUNK`, `Z_MIN_LEVEL`, `Z_MIN_MEMLEVEL`, `Z_MIN_WINDOWBITS`, `Z_NEED_DICT`, `Z_NO_COMPRESSION`, `Z_NO_FLUSH`, `Z_OK`, `Z_PARTIAL_FLUSH`, `Z_RLE`, `Z_STREAM_END`, `Z_STREAM_ERROR`, `Z_SYNC_FLUSH`, `Z_VERSION_ERROR`, `ZLIB_VERNUM`, `ZSTD_btlazy2`, `ZSTD_btopt`, `ZSTD_btultra`, `ZSTD_btultra2`, `ZSTD_c_chainLog`, `ZSTD_c_checksumFlag`, `ZSTD_c_compressionLevel`, `ZSTD_c_contentSizeFlag`, `ZSTD_c_dictIDFlag`, `ZSTD_c_enableLongDistanceMatching`, `ZSTD_c_hashLog`, `ZSTD_c_jobSize`, `ZSTD_c_ldmBucketSizeLog`, `ZSTD_c_ldmHashLog`, `ZSTD_c_ldmHashRateLog`, `ZSTD_c_ldmMinMatch`, `ZSTD_c_minMatch`, `ZSTD_c_nbWorkers`, `ZSTD_c_overlapLog`, `ZSTD_c_searchLog`, `ZSTD_c_strategy`, `ZSTD_c_targetLength`, `ZSTD_c_windowLog`, `ZSTD_CLEVEL_DEFAULT`, `ZSTD_COMPRESS`, `ZSTD_d_windowLogMax`, `ZSTD_DECOMPRESS`, `ZSTD_dfast`, `ZSTD_e_continue`, `ZSTD_e_end`, `ZSTD_e_flush`, `ZSTD_error_checksum_wrong`, `ZSTD_error_corruption_detected`, `ZSTD_error_dictionary_corrupted`, `ZSTD_error_dictionary_wrong`, `ZSTD_error_dictionaryCreation_failed`, `ZSTD_error_dstBuffer_null`, `ZSTD_error_dstSize_tooSmall`, `ZSTD_error_frameParameter_unsupported`, `ZSTD_error_frameParameter_windowTooLarge`, `ZSTD_error_GENERIC`, `ZSTD_error_init_missing`, `ZSTD_error_literals_headerWrong`, `ZSTD_error_maxSymbolValue_tooLarge`, `ZSTD_error_maxSymbolValue_tooSmall`, `ZSTD_error_memory_allocation`, `ZSTD_error_no_error`, `ZSTD_error_noForwardProgress_destFull`, `ZSTD_error_noForwardProgress_inputEmpty`, `ZSTD_error_parameter_combination_unsupported`, `ZSTD_error_parameter_outOfBound`, `ZSTD_error_parameter_unsupported`, `ZSTD_error_prefix_unknown`, `ZSTD_error_srcSize_wrong`, `ZSTD_error_stabilityCondition_notRespected`, `ZSTD_error_stage_wrong`, `ZSTD_error_tableLog_tooLarge`, `ZSTD_error_version_unsupported`, `ZSTD_error_workSpace_tooSmall`, `ZSTD_fast`, `ZSTD_greedy`, `ZSTD_lazy`, `ZSTD_lazy2`, `zstdCompress`, `ZstdCompress`, `zstdCompressSync`, `zstdDecompress`, `ZstdDecompress`, `zstdDecompressSync`

### https

- Coverage: **1 / 6** (16.7%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (5)

- `Agent`, `createServer`, `get`, `globalAgent`, `request`

### process

- Coverage: **15 / 86** (17.4%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (71)

- `_debugEnd`, `_debugProcess`, `_events`, `_eventsCount`, `_exiting`, `_fatalException`, `_getActiveHandles`, `_getActiveRequests`, `_kill`, `_linkedBinding`, `_maxListeners`, `_preload_modules`, `_rawDebug`, `_startProfilerIdleNotifier`, `_stopProfilerIdleNotifier`, `_tickCallback`, `abort`, `allowedNodeEnvironmentFlags`, `availableMemory`, `binding`, `config`, `constrainedMemory`, `cpuUsage`, `debugPort`, `dlopen`, `domain`, `emit`, `emitWarning`, `execArgv`, `execve`, `features`, `finalization`, `getActiveResourcesInfo`, `getBuiltinModule`, `getegid`, `geteuid`, `getgid`, `getgroups`, `getuid`, `hasUncaughtExceptionCaptureCallback`, `hrtime`, `initgroups`, `listenerCount`, `listeners`, `loadEnvFile`, `mainModule`, `memoryUsage`, `moduleLoadList`, `nextTick`, `openStdin`, `reallyExit`, `ref`, `release`, `report`, `resourceUsage`, `setegid`, `seteuid`, `setgid`, `setgroups`, `setSourceMapsEnabled`, `setuid`, `setUncaughtExceptionCaptureCallback`, `sourceMapsEnabled`, `stderr`, `stdin`, `stdout`, `threadCpuUsage`, `title`, `umask`, `unref`, `uptime`

### util

- Coverage: **10 / 34** (29.4%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (24)

- `_errnoException`, `_exceptionWithHostPort`, `_extend`, `aborted`, `callbackify`, `debug`, `diff`, `getCallSite`, `getCallSites`, `getSystemErrorMap`, `getSystemErrorMessage`, `getSystemErrorName`, `MIMEParams`, `MIMEType`, `parseArgs`, `parseEnv`, `promisify`, `setTraceSigInt`, `styleText`, `TextDecoder`, `TextEncoder`, `transferableAbortController`, `transferableAbortSignal`, `types`

### stream

- Coverage: **7 / 23** (30.4%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (16)

- `_isArrayBufferView`, `_isUint8Array`, `_uint8ArrayToBuffer`, `addAbortSignal`, `compose`, `destroy`, `duplexPair`, `getDefaultHighWaterMark`, `isDestroyed`, `isDisturbed`, `isErrored`, `isReadable`, `isWritable`, `PassThrough`, `setDefaultHighWaterMark`, `Transform`

### readline/promises

- Coverage: **1 / 3** (33.3%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (2)

- `createInterface`, `Readline`

### fs

- Coverage: **49 / 108** (45.4%)
- Note: Missing exports are runtime surface differences (names only).
- Note: Extra CLR members may be intentional convenience APIs.

#### Missing exports (59)

- `_toUnixTimestamp`, `chown`, `chownSync`, `constants`, `createReadStream`, `createWriteStream`, `Dir`, `Dirent`, `exists`, `F_OK`, `fchmod`, `fchmodSync`, `fchown`, `fchownSync`, `fdatasync`, `fdatasyncSync`, `FileReadStream`, `FileWriteStream`, `fsync`, `fsyncSync`, `ftruncate`, `ftruncateSync`, `futimes`, `futimesSync`, `glob`, `globSync`, `lchmod`, `lchmodSync`, `lchown`, `lchownSync`, `link`, `linkSync`, `lstat`, `lstatSync`, `lutimes`, `lutimesSync`, `mkdtemp`, `mkdtempDisposableSync`, `mkdtempSync`, `openAsBlob`, `opendir`, `opendirSync`, `R_OK`, `ReadStream`, `readv`, `readvSync`, `statfs`, `statfsSync`, `unwatchFile`, `Utf8Stream`, `utimes`, `utimesSync`, `W_OK`, `watch`, `watchFile`, `WriteStream`, `writev`, `writevSync`, `X_OK`

#### CLR-only members (4)

- `readFileBytes`, `readFileSyncBytes`, `writeFileBytes`, `writeFileSyncBytes`

### perf_hooks

- Coverage: **5 / 11** (45.5%)
- Note: Missing exports are runtime surface differences (names only).
- Note: Extra CLR members may be intentional convenience APIs.

#### Missing exports (6)

- `constants`, `createHistogram`, `monitorEventLoopDelay`, `performance`, `Performance`, `PerformanceResourceTiming`

#### CLR-only members (8)

- `clearMarks`, `clearMeasures`, `getEntries`, `getEntriesByName`, `getEntriesByType`, `mark`, `measure`, `now`

### dns

- Coverage: **23 / 50** (46.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (27)

- `ADDRCONFIG`, `ADDRGETNETWORKPARAMS`, `ALL`, `BADFAMILY`, `BADFLAGS`, `BADHINTS`, `BADNAME`, `BADQUERY`, `BADRESP`, `BADSTR`, `CANCELLED`, `CONNREFUSED`, `DESTRUCTION`, `EOF`, `FILE`, `FORMERR`, `LOADIPHLPAPI`, `NODATA`, `NOMEM`, `NONAME`, `NOTFOUND`, `NOTIMP`, `NOTINITIALIZED`, `REFUSED`, `SERVFAIL`, `TIMEOUT`, `V4MAPPED`

### http

- Coverage: **10 / 20** (50.0%)
- Note: Missing exports are runtime surface differences (names only).
- Note: Extra CLR members may be intentional convenience APIs.

#### Missing exports (10)

- `_connectionListener`, `Agent`, `CloseEvent`, `globalAgent`, `MessageEvent`, `METHODS`, `OutgoingMessage`, `setMaxIdleHTTPParsers`, `STATUS_CODES`, `WebSocket`

#### CLR-only members (3)

- `globalAgent_maxFreeSockets`, `globalAgent_maxSockets`, `globalAgent_timeout`

### tls

- Coverage: **9 / 18** (50.0%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (9)

- `CLIENT_RENEG_LIMIT`, `CLIENT_RENEG_WINDOW`, `convertALPNProtocols`, `DEFAULT_CIPHERS`, `DEFAULT_ECDH_CURVE`, `DEFAULT_MAX_VERSION`, `DEFAULT_MIN_VERSION`, `rootCertificates`, `SecureContext`

### crypto

- Coverage: **47 / 73** (64.4%)
- Note: Missing exports are runtime surface differences (names only).
- Note: Extra CLR members may be intentional convenience APIs.

#### Missing exports (26)

- `argon2`, `argon2Sync`, `checkPrime`, `checkPrimeSync`, `Cipheriv`, `constants`, `createDiffieHellmanGroup`, `decapsulate`, `Decipheriv`, `diffieHellman`, `DiffieHellmanGroup`, `encapsulate`, `fips`, `generateKeySync`, `generatePrime`, `generatePrimeSync`, `getCipherInfo`, `getRandomValues`, `prng`, `pseudoRandomBytes`, `rng`, `secureHeapUsed`, `setEngine`, `subtle`, `webcrypto`, `X509Certificate`

#### CLR-only members (2)

- `getDefaultCipherList`, `setDefaultEncoding`

### dgram

- Coverage: **2 / 3** (66.7%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (1)

- `_createSocketHandle`

### events

- Coverage: **12 / 17** (70.6%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (5)

- `EventEmitterAsyncResource`, `init`, `kMaxEventTargetListeners`, `kMaxEventTargetListenersWarned`, `usingDomains`

### os

- Coverage: **17 / 23** (73.9%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (6)

- `constants`, `devNull`, `EOL`, `getPriority`, `networkInterfaces`, `setPriority`

### console

- Coverage: **23 / 31** (74.2%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (8)

- `_ignoreErrors`, `_stderr`, `_stderrErrorHandler`, `_stdout`, `_stdoutErrorHandler`, `_times`, `context`, `createTask`

### net

- Coverage: **13 / 17** (76.5%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (4)

- `_createServerHandle`, `_normalizeArgs`, `BlockList`, `SocketAddress`

### path

- Coverage: **14 / 17** (82.4%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (3)

- `_makeLong`, `delimiter`, `sep`

### buffer

- Coverage: **12 / 14** (85.7%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (2)

- `Blob`, `File`

### querystring

- Coverage: **6 / 7** (85.7%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (1)

- `unescapeBuffer`

### url

- Coverage: **12 / 14** (85.7%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (2)

- `resolveObject`, `Url`

### assert

- Coverage: **19 / 22** (86.4%)
- Note: Missing exports are runtime surface differences (names only).

#### Missing exports (3)

- `Assert`, `CallTracker`, `partialDeepStrictEqual`

### child_process

- Coverage: **8 / 9** (88.9%)
- Note: Missing exports are runtime surface differences (names only).
- Note: Extra CLR members may be intentional convenience APIs.

#### Missing exports (1)

- `_forkChild`

#### CLR-only members (1)

- `spawnSyncString`

### readline

- Coverage: **8 / 8** (100.0%)
- Note: Extra CLR members may be intentional convenience APIs.

#### CLR-only members (1)

- `createAsyncIterator`

### timers

- Coverage: **7 / 7** (100.0%)
- Note: Extra CLR members may be intentional convenience APIs.

#### CLR-only members (1)

- `queueMicrotask`
