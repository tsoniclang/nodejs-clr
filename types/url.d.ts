/**
 * The url module provides utilities for URL resolution and parsing.
 *
 * @see [Node.js URL Documentation](https://nodejs.org/api/url.html)
 */

export class URLSearchParams {
    /**
     * Gets the total number of parameter entries.
     */
    readonly size: number;

    /**
     * Creates a new URLSearchParams object.
     */
    constructor(init?: string);

    /**
     * Appends a specified key/value pair as a new search parameter.
     */
    append(name: string, value: string): void;

    /**
     * Sets the value associated with a given search parameter.
     */
    set(name: string, value: string): void;

    /**
     * Returns the first value associated with the given search parameter.
     */
    get(name: string): string | null;

    /**
     * Returns all the values associated with a given search parameter.
     */
    getAll(name: string): string[];

    /**
     * Returns a boolean indicating if such a search parameter exists.
     */
    has(name: string, value?: string): boolean;

    /**
     * Deletes the given search parameter and all its associated values.
     */
    delete(name: string, value?: string): void;

    /**
     * Sorts all key/value pairs by their keys.
     */
    sort(): void;

    /**
     * Returns an iterator allowing iteration through all keys.
     */
    keys(): IterableIterator<string>;

    /**
     * Returns an iterator allowing iteration through all values.
     */
    values(): IterableIterator<string>;

    /**
     * Returns an iterator allowing iteration through all key/value pairs.
     */
    entries(): IterableIterator<[string, string]>;

    /**
     * Executes a provided function once for each key/value pair.
     */
    forEach(callback: (value: string, key: string) => void): void;

    /**
     * Returns a query string suitable for use in a URL.
     */
    toString(): string;
}

export class URL {
    /**
     * Creates a new URL object by parsing the input relative to the base.
     */
    constructor(input: string, base?: string);

    /**
     * Gets or sets the serialized URL.
     */
    href: string;

    /**
     * Gets or sets the protocol scheme of the URL.
     */
    protocol: string;

    /**
     * Gets or sets the username portion of the URL.
     */
    username: string;

    /**
     * Gets or sets the password portion of the URL.
     */
    password: string;

    /**
     * Gets or sets the host portion of the URL (hostname + port).
     */
    host: string;

    /**
     * Gets or sets the hostname portion of the URL.
     */
    hostname: string;

    /**
     * Gets or sets the port portion of the URL.
     */
    port: string;

    /**
     * Gets or sets the path portion of the URL.
     */
    pathname: string;

    /**
     * Gets or sets the serialized query portion of the URL.
     */
    search: string;

    /**
     * Gets a URLSearchParams object representing the query parameters.
     */
    readonly searchParams: URLSearchParams;

    /**
     * Gets or sets the fragment portion of the URL.
     */
    hash: string;

    /**
     * Gets the read-only origin of the URL.
     */
    readonly origin: string;

    /**
     * Returns the serialized URL as a string.
     */
    toString(): string;

    /**
     * Returns the serialized URL as a string (for JSON serialization).
     */
    toJSON(): string;

    /**
     * Tests if input can be parsed as a URL.
     */
    static canParse(input: string, base?: string): boolean;

    /**
     * Parses a URL string and returns a URL object, or null if parsing fails.
     */
    static parse(input: string, base?: string): URL | null;
}
