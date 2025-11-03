/**
 * The `process` object provides information about, and control over, the current Node.js process.
 */

/**
 * An object containing the version strings of Node.js and its dependencies.
 */
export interface ProcessVersions {
    node: string;
    v8: string;
    dotnet: string;
    tsonic: string;
}

/**
 * An object containing the user environment.
 */
export interface ProcessEnv {
    [key: string]: string | undefined;
}

/**
 * The process object provides information about, and control over, the current Node.js process.
 */
export interface Process {
    /**
     * The `process.argv` property returns an array containing the command-line arguments
     * passed when the Node.js process was launched.
     */
    argv: string[];

    /**
     * The `process.argv0` property stores a read-only copy of the original value of `argv[0]`
     * passed when Node.js starts.
     */
    argv0: string;

    /**
     * The `process.env` property returns an object containing the user environment.
     */
    env: ProcessEnv;

    /**
     * The operating system platform on which the Node.js process is running.
     * Possible values are: 'aix', 'darwin', 'freebsd', 'linux', 'openbsd', 'sunos', 'win32'.
     */
    readonly platform: string;

    /**
     * The operating system CPU architecture for which the Node.js binary was compiled.
     * Possible values are: 'arm', 'arm64', 'ia32', 'loong64', 'mips', 'mipsel', 'ppc',
     * 'ppc64', 'riscv64', 's390', 's390x', and 'x64'.
     */
    readonly arch: string;

    /**
     * The Node.js version string.
     */
    readonly version: string;

    /**
     * An object containing the version strings of Node.js and its dependencies.
     */
    readonly versions: ProcessVersions;

    /**
     * The `process.execPath` property returns the absolute pathname of the executable
     * that started the process.
     */
    readonly execPath: string;

    /**
     * The PID of the current process.
     */
    readonly pid: number;

    /**
     * The PID of the parent process.
     */
    readonly ppid: number;

    /**
     * A number which will be the process exit code, when the process either exits gracefully,
     * or is exited via `process.exit()` without specifying a code.
     */
    exitCode: number | undefined;

    /**
     * The `process.cwd()` method returns the current working directory of the Node.js process.
     */
    cwd(): string;

    /**
     * The `process.chdir()` method changes the current working directory of the Node.js process
     * or throws an exception if doing so fails (for instance, if the specified directory does not exist).
     */
    chdir(directory: string): void;

    /**
     * The `process.exit()` method instructs Node.js to terminate the process synchronously
     * with an exit status of `code`. If `code` is omitted, exit uses either the 'success' code `0`
     * or the value of `process.exitCode` if it has been set.
     */
    exit(code?: number): never;

    /**
     * The `process.kill()` method sends the signal to the process identified by `pid`.
     * Signal names are strings such as `'SIGINT'` or `'SIGHUP'`. If omitted, the signal defaults to `'SIGTERM'`.
     * This method will throw an error if the target `pid` does not exist.
     */
    kill(pid: number, signal?: string | number): true;
}

declare const process: Process;
export default process;

// Named exports for all properties and methods
export const argv: Process["argv"];
export const argv0: Process["argv0"];
export const env: Process["env"];
export const platform: Process["platform"];
export const arch: Process["arch"];
export const version: Process["version"];
export const versions: Process["versions"];
export const execPath: Process["execPath"];
export const pid: Process["pid"];
export const ppid: Process["ppid"];
export const exitCode: Process["exitCode"];
export const cwd: Process["cwd"];
export const chdir: Process["chdir"];
export const exit: Process["exit"];
export const kill: Process["kill"];
