/**
 * Much of the Node.js core API is built around an idiomatic asynchronous
 * event-driven architecture. This is a simplified EventEmitter implementation.
 */
declare module "events" {
    /**
     * The EventEmitter class is used for handling events.
     */
    export class EventEmitter {
        /** Default maximum number of listeners for all EventEmitter instances. */
        static defaultMaxListeners: number;

        /**
         * Adds a listener function to the end of the listeners array for the specified event.
         * @param eventName The name of the event.
         * @param listener The callback function.
         * @returns This EventEmitter instance for chaining.
         */
        on(eventName: string, listener: (...args: any[]) => void): this;

        /**
         * Adds a listener function that will be invoked only once for the specified event.
         * @param eventName The name of the event.
         * @param listener The callback function.
         * @returns This EventEmitter instance for chaining.
         */
        once(eventName: string, listener: (...args: any[]) => void): this;

        /**
         * Synchronously calls each of the listeners registered for the event,
         * in the order they were registered, passing the supplied arguments to each.
         * @param eventName The name of the event.
         * @param args Arguments to pass to the listeners.
         * @returns True if the event had listeners, false otherwise.
         */
        emit(eventName: string, ...args: any[]): boolean;

        /**
         * Removes the specified listener from the listener array for the event.
         * @param eventName The name of the event.
         * @param listener The callback function to remove.
         * @returns This EventEmitter instance for chaining.
         */
        removeListener(eventName: string, listener: (...args: any[]) => void): this;

        /**
         * Alias for removeListener().
         */
        off(eventName: string, listener: (...args: any[]) => void): this;

        /**
         * Removes all listeners, or those of the specified eventName.
         * @param eventName Optional event name.
         * @returns This EventEmitter instance for chaining.
         */
        removeAllListeners(eventName?: string): this;

        /**
         * Returns a copy of the array of listeners for the event named eventName.
         * @param eventName The name of the event.
         * @returns Array of listener functions.
         */
        listeners(eventName: string): Function[];

        /**
         * Returns the number of listeners listening to the event named eventName.
         * @param eventName The name of the event.
         * @returns The number of listeners.
         */
        listenerCount(eventName: string): number;

        /**
         * Returns an array listing the events for which the emitter has registered listeners.
         * @returns Array of event names.
         */
        eventNames(): string[];

        /**
         * Sets the maximum number of listeners that can be added for a single event.
         * @param n The maximum number of listeners. Set to 0 for unlimited.
         * @returns This EventEmitter instance for chaining.
         */
        setMaxListeners(n: number): this;

        /**
         * Returns the current maximum listener value for this EventEmitter.
         * @returns The maximum number of listeners.
         */
        getMaxListeners(): number;

        /**
         * Adds a listener to the beginning of the listeners array for the specified event.
         * @param eventName The name of the event.
         * @param listener The callback function.
         * @returns This EventEmitter instance for chaining.
         */
        prependListener(eventName: string, listener: (...args: any[]) => void): this;

        /**
         * Adds a one-time listener to the beginning of the listeners array.
         * @param eventName The name of the event.
         * @param listener The callback function.
         * @returns This EventEmitter instance for chaining.
         */
        prependOnceListener(eventName: string, listener: (...args: any[]) => void): this;

        /**
         * Alias for on(). Adds a listener to the end of the listeners array.
         */
        addListener(eventName: string, listener: (...args: any[]) => void): this;

        /**
         * Returns a copy of the array of listeners for the event, including any wrappers.
         * @param eventName The name of the event.
         * @returns Array of listener functions.
         */
        rawListeners(eventName: string): Function[];
    }

    export default EventEmitter;
}

declare module "node:events" {
    export * from "events";
}
