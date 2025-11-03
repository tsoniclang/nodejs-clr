/**
 * The assert module provides a set of assertion functions for verifying invariants.
 *
 * @see [Node.js Assert Documentation](https://nodejs.org/api/assert.html)
 */

export class AssertionError extends Error {
    actual: unknown;
    expected: unknown;
    operator: string;
    generatedMessage: boolean;
    code: "ERR_ASSERTION";

    constructor(message?: string, actual?: unknown, expected?: unknown, operator?: string);
}

/**
 * Tests if value is truthy.
 */
export function ok(value: unknown, message?: string): void;

/**
 * Always fails with the provided message.
 */
export function fail(message?: string): never;

/**
 * Tests shallow, coercive equality between actual and expected using ==.
 */
export function equal(actual: unknown, expected: unknown, message?: string): void;

/**
 * Tests shallow, coercive inequality between actual and expected using !=.
 */
export function notEqual(actual: unknown, expected: unknown, message?: string): void;

/**
 * Tests strict equality between actual and expected using ===.
 */
export function strictEqual(actual: unknown, expected: unknown, message?: string): void;

/**
 * Tests strict inequality between actual and expected using !==.
 */
export function notStrictEqual(actual: unknown, expected: unknown, message?: string): void;

/**
 * Tests for deep equality between actual and expected.
 */
export function deepEqual(actual: unknown, expected: unknown, message?: string): void;

/**
 * Tests for deep inequality between actual and expected.
 */
export function notDeepEqual(actual: unknown, expected: unknown, message?: string): void;

/**
 * Tests for deep strict equality between actual and expected.
 */
export function deepStrictEqual(actual: unknown, expected: unknown, message?: string): void;

/**
 * Tests for deep strict inequality between actual and expected.
 */
export function notDeepStrictEqual(actual: unknown, expected: unknown, message?: string): void;

/**
 * Expects the function fn to throw an error.
 */
export function throws(fn: () => void, message?: string): void;

/**
 * Expects the function fn not to throw an error.
 */
export function doesNotThrow(fn: () => void, message?: string): void;

/**
 * Expects the string input to match the regular expression.
 */
export function match(string: string, regexp: RegExp, message?: string): void;

/**
 * Expects the string input not to match the regular expression.
 */
export function doesNotMatch(string: string, regexp: RegExp, message?: string): void;

/**
 * Tests if value is not null or undefined (throws if it is).
 */
export function ifError(value: unknown): void;
