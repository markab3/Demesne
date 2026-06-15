using Godot;
using System.Text.Json.Nodes;

namespace Demesne.Client.Autoloads;

public partial class StateManager : Node
{
    [Signal] public delegate void TilesChangedEventHandler(string[] tileKeys);
    [Signal] public delegate void ResourcesChangedEventHandler();
    [Signal] public delegate void PopulationChangedEventHandler();
    [Signal] public delegate void TickAdvancedEventHandler(int newTick);
    [Signal] public delegate void EventsChangedEventHandler();

    private JsonObject _state = [];

    // Array merge strategy by JSON key — see docs/design/frontend.md
    private static readonly Dictionary<string, string> ArrayStrategies = new()
    {
        ["events"] = "AppendNew",
        ["units"]  = "MergeById",
        ["effects"] = "Replace",
    };

    public void MergePartial(JsonObject delta)
    {
        var changedTileKeys = new List<string>();
        Merge(_state, delta, changedTileKeys);
        if (changedTileKeys.Count > 0)
            EmitSignal(SignalName.TilesChanged, [changedTileKeys.ToArray()]);
    }

    private void Merge(JsonObject target, JsonObject source, List<string> changedTileKeys)
    {
        foreach (var (key, sourceNode) in source)
        {
            if (sourceNode is JsonObject sourceObj && target[key] is JsonObject targetObj)
            {
                Merge(targetObj, sourceObj, changedTileKeys);
                if (key == "tiles")
                    changedTileKeys.AddRange(sourceObj.Select(p => p.Key));
            }
            else if (sourceNode is JsonArray sourceArr && ArrayStrategies.TryGetValue(key, out var strategy))
            {
                MergeArray(target, key, sourceArr, strategy);
            }
            else
            {
                target[key] = sourceNode?.DeepClone();
            }
        }
    }

    private static void MergeArray(JsonObject target, string key, JsonArray source, string strategy)
    {
        switch (strategy)
        {
            case "Replace":
                target[key] = source.DeepClone();
                break;

            case "AppendNew":
                var existing = target[key] as JsonArray ?? [];
                var seen = existing.OfType<JsonObject>()
                    .Select(o => o["id"]?.GetValue<string>())
                    .Where(id => id is not null)
                    .ToHashSet();
                foreach (var item in source)
                    if (item is JsonObject obj && obj["id"]?.GetValue<string>() is string id && seen.Add(id))
                        existing.Add(item.DeepClone());
                target[key] = existing;
                break;

            case "MergeById":
                var arr = target[key] as JsonArray ?? [];
                var byId = arr.OfType<JsonObject>()
                    .Where(o => o["id"]?.GetValue<string>() is not null)
                    .ToDictionary(o => o["id"]!.GetValue<string>(), o => o);
                var dummy = new List<string>();
                foreach (var item in source.OfType<JsonObject>())
                {
                    if (item["id"]?.GetValue<string>() is string id)
                    {
                        if (byId.TryGetValue(id, out var existingObj))
                            Merge(existingObj, item, dummy);
                        else
                            arr.Add(item.DeepClone());
                    }
                }
                target[key] = arr;
                break;
        }
    }
}
