namespace Tsonic.NodeApi;

/// <summary>
/// Implements Node.js EventEmitter functionality.
/// All methods follow JavaScript naming conventions (lowercase).
/// </summary>
public partial class EventEmitter
{
    private readonly Dictionary<string, List<Delegate>> _events = new();
    private readonly Dictionary<string, List<Delegate>> _onceWrappers = new();
    private int _maxListeners = 10;
    private static int _defaultMaxListeners = 10;

    /// <summary>
    /// Gets or sets the default maximum number of listeners for all EventEmitter instances.
    /// </summary>
    public static int defaultMaxListeners
    {
        get => _defaultMaxListeners;
        set => _defaultMaxListeners = value;
    }
}
