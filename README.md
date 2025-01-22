# :warning: Deprecation Warning :warning:

This library has been deprecated and will no longer receive updates.

---

RockLib has been a cornerstone of our open source efforts here at Rocket Mortgage, and it's played a significant role in our journey to drive innovation and collaboration within our organization and the open source community. It's been amazing to witness the collective creativity and hard work that you all have poured into this project.

However, as technology relentlessly evolves, so must we. The decision to deprecate this library is rooted in our commitment to staying at the cutting edge of technological advancements. While this chapter is ending, it opens the door to exciting new opportunities on the horizon.

We want to express our heartfelt thanks to all the contributors and users who have been a part of this incredible journey. Your contributions, feedback, and enthusiasm have been invaluable, and we are genuinely grateful for your dedication. 🚀

---

# RockLib.Collections

```powershell
PM> Install-Package RockLib.Collections
```

### `NamedCollection<T>`

This class (and the `ToNamedCollection` extension method) are useful as an implementation detail of a class that has an API where callers (optionally) specify a name, and the class looks up some value according to that name. If the name is null, empty, or the string "default", then the `NamedCollection<T>` returns its default value, which is a value whose name is null, empty, or the string "default".

Example:

```csharp
public class Worker
{
    private readonly NamedCollection<Detail> _details;

    public Worker(IEnumerable<Detail> details)
    {
        _details = details.ToNamedCollection(d => d.Name);
    }

    public void DoWork(string detailName = null)
    {
        if (!_details.TryGetValue(detailName, out Detail detail))
            throw new KeyNotFoundException($"detailName not found: {detailName}.");

        // TODO: Do something with the detail.
    }
}

public class Detail
{
    public Detail(string name) => Name = name;
    public string Name { get; }
}
```

 ### Supported Targets

 This library supports the following targets:
   - .NET 6
   - .NET Core 3.1
   - .NET Framework 4.8
