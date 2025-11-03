/**
 * The timers module exposes a global API for scheduling functions to be called at some future period of time.
 *
 * @see [Node.js Timers Documentation](https://nodejs.org/api/timers.html)
 */

export class Timeout {
    /**
     * Requests that the Node.js event loop not exit so long as the Timeout is active.
     */
    ref(): this;

    /**
     * Allows the Node.js event loop to exit if this is the only active handle.
     */
    unref(): this;

    /**
     * Returns true if the timer will keep the event loop active.
     */
    hasRef(): boolean;

    /**
     * Restarts the timer.
     */
    refresh(): this;

    /**
     * Cancels the timeout.
     */
    close(): void;
}

export class Immediate {
    /**
     * Requests that the Node.js event loop not exit so long as the Immediate is active.
     */
    ref(): this;

    /**
     * Allows the Node.js event loop to exit if this is the only active handle.
     */
    unref(): this;

    /**
     * Returns true if the immediate will keep the event loop active.
     */
    hasRef(): boolean;
}

/**
 * Schedules execution of a one-time callback after delay milliseconds.
 */
export function setTimeout(callback: () => void, delay?: number): Timeout;

/**
 * Cancels a Timeout object created by setTimeout().
 */
export function clearTimeout(timeout?: Timeout): void;

/**
 * Schedules repeated execution of callback every delay milliseconds.
 */
export function setInterval(callback: () => void, delay?: number): Timeout;

/**
 * Cancels a Timeout object created by setInterval().
 */
export function clearInterval(timeout?: Timeout): void;

/**
 * Schedules the immediate execution of callback after I/O events callbacks.
 */
export function setImmediate(callback: () => void): Immediate;

/**
 * Cancels an Immediate object created by setImmediate().
 */
export function clearImmediate(immediate?: Immediate): void;

/**
 * Queues a microtask to invoke callback.
 */
export function queueMicrotask(callback: () => void): void;
