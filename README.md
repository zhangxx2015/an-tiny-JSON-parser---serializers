# an-tiny-JSON-parser-and-serializers
## An tiny JSON parser & serializers, only 154 lines of code

### Example

``` csharp
var json = Json.ToJson(new { Name = "james", Age = 35, Married = true });
var jobj = (Dictionary<string, object>)Json.FromJson(json);
foreach (var key in jobj.Keys) {
    Console.WriteLine("\t{0}:{1}", key, jobj[key]);
}
```
