# Ao.SavableConfig
[![.NET](https://github.com/Cricle/Ao.SavableConfig/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Cricle/Ao.SavableConfig/actions/workflows/dotnet.yml)
[![Github lines](https://img.shields.io/tokei/lines/github/Cricle/Ao.SavableConfig)](https://github.com/Cricle/Ao.SavableConfig)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/28d777d354ed4984ad988703b1094665)](https://www.codacy.com/gh/Cricle/Ao.SavableConfig/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Cricle/Ao.SavableConfig&amp;utm_campaign=Badge_Grade)Cancel changes
[![CodeQL](https://github.com/Cricle/Ao.SavableConfig/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Cricle/Ao.SavableConfig/actions/workflows/codeql-analysis.yml)
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

Or wpf project `sample\TwoWayBindWpf` use in wpf binding

```csharp
var builder = new SavableConfiurationBuilder();
builder.AddJsonFile("appsettings.json", false, true);
//Make configuration
var root = builder.Build();
//Create proxy and create proxy object
var value = root.AutoCreateProxy<DbConnection>();
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
