## System.Linq.Dynamic.Core
This is a **.NET Core / Standard port** of the Microsoft assembly for the .Net 4.0 Dynamic language functionality.

### Overview
With this library it's possible to write Dynamic LINQ queries (string based) on an `IQueryable`:
``` c#
var query = db.Customers
    .Where("City == @0 and Orders.Count >= @1", "London", 10)
    .OrderBy("CompanyName")
    .Select("new(CompanyName as Name, Phone)");
```

### Useful links

- [Website](https://dynamic-linq.net)
- [Documentation](https://dynamic-linq.net/overview)
- [Online examples](https://dynamic-linq.net/online-examples)


### Library Powered By

This library is powered by [Entity Framework Extensions](https://entityframework-extensions.net/?z=github&y=system.linq.dynamic.core)