/**
 * The `util` module supports the needs of Node.js internal APIs.
 * Many of the utilities are useful for application and module developers as well.
 *
 * ```js
 * import util from 'util';
 * ```
 */

/**
 * Returns a formatted string using the first argument as a printf-like format string.
 * The first argument is a string containing zero or more placeholder tokens.
 * Each placeholder token is replaced with the converted value from the corresponding argument.
 *
 * Supported placeholders are:
 * - %s: String
 * - %d: Number
 * - %i: Integer
 * - %f: Floating point value
 * - %j: JSON
 * - %o: Object
 * - %%: single percent sign ('%'). This does not consume an argument.
 */
export function format(format?: any, ...param: any[]): string;

/**
 * Returns a string representation of an object that is intended for debugging.
 * The returned string represents the object in a human-readable format.
 */
export function inspect(object: any): string;

/**
 * Returns true if the given object is an Array. Otherwise, returns false.
 */
export function isArray(object: unknown): object is unknown[];

/**
 * Returns true if there is deep strict equality between val1 and val2. Otherwise, returns false.
 * Primitive values are compared using Object.is(). Does not compare [[Prototype]] values,
 * and does not compare non-enumerable properties or symbols.
 */
export function isDeepStrictEqual(val1: unknown, val2: unknown): boolean;

/**
 * Inherit the prototype methods from one constructor into another.
 * The prototype of constructor will be set to a new object created from superConstructor.
 *
 * Note: This is primarily for compatibility with JavaScript patterns.
 * Usage of ES6 classes with the extends keyword is preferred.
 */
export function inherits(constructor: unknown, superConstructor: unknown): void;

/**
 * A function used for conditional debug logging.
 */
export type DebugLogFunction = (message: string, ...args: any[]) => void;

/**
 * Creates a function that conditionally writes debug messages to stderr
 * based on the existence of the NODE_DEBUG environment variable.
 * If the section name appears in the value of that environment variable,
 * then the returned function operates similar to console.error().
 * If not, then the returned function is a no-op.
 */
export function debuglog(section: string): DebugLogFunction;

/**
 * Marks a function as deprecated. When the deprecated function is called,
 * a warning is printed to stderr.
 */
export function deprecate<T extends Function>(fn: T, msg: string, code?: string): T;
