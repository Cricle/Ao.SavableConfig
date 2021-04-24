# Ao.SavableConfig

## What it is 

It can two way bind configuration.

The MS configuration provider file -> string map/class properties way to fetch configuration, and the project support string map/class properties -> file way to save the configuration change.
And it can dynamic proxy the configuration, it can bind at wpf, xamarin, uwp... When it use configuration UI!
And it can auto save, when change the configuration!

And it support proxy setting class, simple modify class property to change properties, and auto save the configuration!

## Why it can do that

Can reversal store is i modify the MS configuration lib to make it support, and proxy use MS IL to dynamic create proxy class.

## How to use it

>The `NameTransfer` is very import to provider find configuration path!

You can see `sample\TwoWayBind`

```csharp
var builder = new SavableConfiurationBuilder();
builder.AddJsonFile("appsettings.json", false, true);
//Make configuration
var root = builder.Build();
//Create proxy and create proxy object
var connection = ProxyHelper.Default.CreateComplexProxy<DbConnection>(true);
var value = (DbConnection)connection.Build(root.GetSection("DbConnections"));
//Bind it two way (or `OneWay` , `OneWayToSource` `OneTime`)
root.BindTwoWay(value, JsonChangeTransferCondition.Instance);
```

# After

1. Add unit test, to conver `100%` codes
2. To make easy use it
3. Add object <-> string converter like wpf `IValueConverter`
4. Add more provider transfer (Now only json)

# Nuget 

[Ao.SavableConfig](https://www.nuget.org/packages/Ao.SavableConfig/)
[Ao.SavableConfig.Binder](https://www.nuget.org/packages/Ao.SavableConfig.Binder/)
[Ao.SavableConfig.Json](https://www.nuget.org/packages/Ao.SavableConfig.Json/)