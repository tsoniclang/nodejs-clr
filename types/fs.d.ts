/**
 * The `fs` module enables interacting with the file system.
 * Supports both synchronous and asynchronous (Promise-based) methods.
 */
declare module "fs" {
    /**
     * Provides information about a file or directory.
     */
    export class Stats {
        /** The size of the file in bytes (0 for directories). */
        size: number;
        /** The file mode (permissions). */
        mode: number;
        /** The last access time. */
        atime: Date;
        /** The last modified time. */
        mtime: Date;
        /** The last status change time. */
        ctime: Date;
        /** The creation time (birthtime). */
        birthtime: Date;
        /** True if this is a file. */
        isFile: boolean;
        /** True if this is a directory. */
        isDirectory: boolean;

        /** Returns true if this is a file. */
        IsFile(): boolean;
        /** Returns true if this is a directory. */
        IsDirectory(): boolean;
        /** Returns true if this is a symbolic link. */
        IsSymbolicLink(): boolean;
        /** Returns true if this is a block device. */
        IsBlockDevice(): boolean;
        /** Returns true if this is a character device. */
        IsCharacterDevice(): boolean;
        /** Returns true if this is a FIFO. */
        IsFIFO(): boolean;
        /** Returns true if this is a socket. */
        IsSocket(): boolean;
    }

    /**
     * Synchronously reads the entire contents of a file.
     * @param path Filename or file path.
     * @param encoding Character encoding (e.g., "utf-8"). If null, returns Buffer.
     * @returns The contents of the file as a string.
     */
    export function readFileSync(path: string, encoding?: string | null): string;

    /**
     * Asynchronously reads the entire contents of a file.
     * @param path Filename or file path.
     * @param encoding Character encoding (e.g., "utf-8"). If null, returns Buffer.
     * @returns A promise that resolves to the contents of the file as a string.
     */
    export function readFile(path: string, encoding?: string | null): Promise<string>;

    /**
     * Synchronously writes data to a file, replacing the file if it already exists.
     * @param path Filename or file path.
     * @param data The data to write.
     * @param encoding Character encoding (default: "utf-8").
     */
    export function writeFileSync(path: string, data: string, encoding?: string | null): void;

    /**
     * Asynchronously writes data to a file, replacing the file if it already exists.
     * @param path Filename or file path.
     * @param data The data to write.
     * @param encoding Character encoding (default: "utf-8").
     * @returns A promise that resolves when the write is complete.
     */
    export function writeFile(path: string, data: string, encoding?: string | null): Promise<void>;

    /**
     * Synchronously appends data to a file, creating the file if it does not yet exist.
     * @param path Filename or file path.
     * @param data The data to append.
     * @param encoding Character encoding (default: "utf-8").
     */
    export function appendFileSync(path: string, data: string, encoding?: string | null): void;

    /**
     * Asynchronously appends data to a file, creating the file if it does not yet exist.
     * @param path Filename or file path.
     * @param data The data to append.
     * @param encoding Character encoding (default: "utf-8").
     * @returns A promise that resolves when the append is complete.
     */
    export function appendFile(path: string, data: string, encoding?: string | null): Promise<void>;

    /**
     * Returns true if the path exists, false otherwise.
     * @param path The path to check.
     * @returns True if the path exists.
     */
    export function existsSync(path: string): boolean;

    /**
     * Synchronously creates a directory.
     * @param path The directory path to create.
     * @param recursive If true, creates parent directories as needed (default: false).
     */
    export function mkdirSync(path: string, recursive?: boolean): void;

    /**
     * Asynchronously creates a directory.
     * @param path The directory path to create.
     * @param recursive If true, creates parent directories as needed (default: false).
     * @returns A promise that resolves when the directory is created.
     */
    export function mkdir(path: string, recursive?: boolean): Promise<void>;

    /**
     * Synchronously reads the contents of a directory.
     * @param path The directory path.
     * @param withFileTypes If true, returns directory entries with type info.
     * @returns An array of filenames.
     */
    export function readdirSync(path: string, withFileTypes?: boolean): string[];

    /**
     * Asynchronously reads the contents of a directory.
     * @param path The directory path.
     * @param withFileTypes If true, returns directory entries with type info.
     * @returns A promise that resolves to an array of filenames.
     */
    export function readdir(path: string, withFileTypes?: boolean): Promise<string[]>;

    /**
     * Synchronously retrieves statistics for the file/directory at the given path.
     * @param path The file or directory path.
     * @returns A Stats object.
     */
    export function statSync(path: string): Stats;

    /**
     * Asynchronously retrieves statistics for the file/directory at the given path.
     * @param path The file or directory path.
     * @returns A promise that resolves to a Stats object.
     */
    export function stat(path: string): Promise<Stats>;

    /**
     * Synchronously deletes a file.
     * @param path The file path to delete.
     */
    export function unlinkSync(path: string): void;

    /**
     * Asynchronously deletes a file.
     * @param path The file path to delete.
     * @returns A promise that resolves when the file is deleted.
     */
    export function unlink(path: string): Promise<void>;

    /**
     * Synchronously removes a directory.
     * @param path The directory path to remove.
     * @param recursive If true, removes directory and all contents (default: false).
     */
    export function rmdirSync(path: string, recursive?: boolean): void;

    /**
     * Asynchronously removes a directory.
     * @param path The directory path to remove.
     * @param recursive If true, removes directory and all contents (default: false).
     * @returns A promise that resolves when the directory is removed.
     */
    export function rmdir(path: string, recursive?: boolean): Promise<void>;

    /**
     * Synchronously copies src to dest. By default, dest is overwritten if it already exists.
     * @param src Source filename to copy.
     * @param dest Destination filename.
     * @param mode Optional flags.
     */
    export function copyFileSync(src: string, dest: string, mode?: number): void;

    /**
     * Asynchronously copies src to dest. By default, dest is overwritten if it already exists.
     * @param src Source filename to copy.
     * @param dest Destination filename.
     * @param mode Optional flags.
     * @returns A promise that resolves when the copy is complete.
     */
    export function copyFile(src: string, dest: string, mode?: number): Promise<void>;

    /**
     * Synchronously renames (moves) a file or directory.
     * @param oldPath The old path.
     * @param newPath The new path.
     */
    export function renameSync(oldPath: string, newPath: string): void;

    /**
     * Asynchronously renames (moves) a file or directory.
     * @param oldPath The old path.
     * @param newPath The new path.
     * @returns A promise that resolves when the rename is complete.
     */
    export function rename(oldPath: string, newPath: string): Promise<void>;

    /**
     * Synchronously reads the entire contents of a file as a byte array.
     * @param path Filename or file path.
     * @returns The contents of the file as a byte array.
     */
    export function readFileSyncBytes(path: string): Uint8Array;

    /**
     * Asynchronously reads the entire contents of a file as a byte array.
     * @param path Filename or file path.
     * @returns A promise that resolves to the contents of the file as a byte array.
     */
    export function readFileBytes(path: string): Promise<Uint8Array>;

    /**
     * Synchronously writes a byte array to a file, replacing the file if it already exists.
     * @param path Filename or file path.
     * @param data The byte array to write.
     */
    export function writeFileSyncBytes(path: string, data: Uint8Array): void;

    /**
     * Asynchronously writes a byte array to a file, replacing the file if it already exists.
     * @param path Filename or file path.
     * @param data The byte array to write.
     * @returns A promise that resolves when the write is complete.
     */
    export function writeFileBytes(path: string, data: Uint8Array): Promise<void>;
}

declare module "node:fs" {
    export * from "fs";
}
