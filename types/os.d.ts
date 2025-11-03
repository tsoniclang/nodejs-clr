/**
 * The `os` module provides operating system-related utility methods and properties.
 *
 * ```js
 * import os from 'os';
 * ```
 */

/**
 * Information about a logical CPU core.
 */
export interface CpuInfo {
    /** The CPU model name. */
    model: string;
    /** The CPU speed in MHz. */
    speed: number;
    /** CPU time statistics. */
    times: {
        /** The number of milliseconds the CPU has spent in user mode. */
        user: number;
        /** The number of milliseconds the CPU has spent in nice mode. */
        nice: number;
        /** The number of milliseconds the CPU has spent in sys mode. */
        sys: number;
        /** The number of milliseconds the CPU has spent in idle mode. */
        idle: number;
        /** The number of milliseconds the CPU has spent in irq mode. */
        irq: number;
    };
}

/**
 * Information about the currently effective user.
 */
export interface UserInfo {
    /** The username. */
    username: string;
    /** The user identifier (POSIX only, -1 on Windows). */
    uid: number;
    /** The group identifier (POSIX only, -1 on Windows). */
    gid: number;
    /** The user's shell (POSIX only, null on Windows). */
    shell: string | null;
    /** The user's home directory. */
    homedir: string;
}

/**
 * Returns the host name of the operating system as a string.
 */
export function hostname(): string;

/**
 * Returns an array containing the 1, 5, and 15 minute load averages.
 * The load average is a Unix-specific concept. On Windows, the return value is always [0, 0, 0].
 */
export function loadavg(): number[];

/**
 * Returns the system uptime in number of seconds.
 */
export function uptime(): number;

/**
 * Returns the amount of free system memory in bytes as an integer.
 */
export function freemem(): number;

/**
 * Returns the total amount of system memory in bytes as an integer.
 */
export function totalmem(): number;

/**
 * Returns an array of objects containing information about each logical CPU core.
 */
export function cpus(): CpuInfo[];

/**
 * Returns an estimate of the default amount of parallelism a program should use.
 * Always returns a value greater than zero.
 */
export function availableParallelism(): number;

/**
 * Returns the operating system name as returned by uname(3).
 * For example, it returns 'Linux' on Linux, 'Darwin' on macOS, and 'Windows_NT' on Windows.
 */
export function type(): string;

/**
 * Returns the operating system release version as a string.
 */
export function release(): string;

/**
 * Returns the string path of the current user's home directory.
 */
export function homedir(): string;

/**
 * Returns information about the currently effective user.
 */
export function userInfo(): UserInfo;

/**
 * Returns the operating system's default directory for temporary files as a string.
 */
export function tmpdir(): string;

/**
 * Returns a string identifying the endianness of the CPU.
 * Possible values are 'BE' for big endian and 'LE' for little endian.
 */
export function endianness(): "BE" | "LE";

/**
 * Returns the operating system CPU architecture for which the Node.js binary was compiled.
 * Possible values are 'arm', 'arm64', 'ia32', 'x64', etc.
 */
export function arch(): string;

/**
 * Returns a string identifying the operating system platform.
 * Possible values are 'aix', 'darwin', 'freebsd', 'linux', 'openbsd', 'sunos', and 'win32'.
 */
export function platform(): string;

/**
 * The operating system-specific end-of-line marker.
 * \\n on POSIX
 * \\r\\n on Windows
 */
export const EOL: string;

/**
 * The platform-specific path to the null device.
 * /dev/null on POSIX
 * \\\\.\\nul on Windows
 */
export const devNull: string;
