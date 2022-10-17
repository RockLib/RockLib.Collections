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
