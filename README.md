<div align='center' >
<h1>SavableConfig</h1>
</div>

<div align='center' >
	<h5>Can bind two way configuration</h5>
</div>

<div align='center'>

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/28d777d354ed4984ad988703b1094665)](https://www.codacy.com/gh/Cricle/Ao.SavableConfig/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Cricle/Ao.SavableConfig&amp;utm_campaign=Badge_Grade)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FCricle%2FAo.SavableConfig.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FCricle%2FAo.SavableConfig?ref=badge_shield)
[![codecov](https://codecov.io/gh/Cricle/Ao.SavableConfig/branch/master/graph/badge.svg?token=VI05YYQH2w)](https://codecov.io/gh/Cricle/Ao.SavableConfig)

</div>

## Build Status

|Provider|Status|
|:-:|:-|
|Github|[![.NET](https://github.com/Cricle/Ao.SavableConfig/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Cricle/Ao.SavableConfig/actions/workflows/dotnet.yml)|
|Azure Pipelines|[![Build Status](https://hcricle.visualstudio.com/Ao.SavableConfig/_apis/build/status/Cricle.Ao.SavableConfig?branchName=master)](https://hcricle.visualstudio.com/Ao.SavableConfig/_build/latest?definitionId=8&branchName=master)|

## What it is 

It can two way bind configuration.

The MS configurations provider `File -> Map` way.

The project support `Map changed -> File` way.

- It support configuration dynamic proxy.
- It can use data binding at wpf, xamarin, avaloniaui...
- It can auto save, when the configuration is changed!
- It can auto proxy your profile class, modify property auto modify configuration.

## Why it can do that

Can reversal store is i modify the MS configuration lib to make it support, and proxy use MS IL to dynamic create proxy class.

## How to use it

>The `NameTransfer` is very import to provider find configuration path!

You can watch `sample\TwoWayBind`

Or wpf project `sample\TwoWayBindWpf` use in wpf binding

```csharp
var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json", false, true);
//Make configuration
var root = builder.BuildSavable();
//Create proxy and create proxy object
var value = root.AutoCreateProxy<DbConnection>();
root.BindTwoWay(value, JsonChangeTransferCondition.Instance);
```

# Features

- [x] Add unit test, to conver nearly `100%` codes
- [x] To make easy use it
- [ ] Add object <-> string converter like wpf `IValueConverter`
- [ ] Add more provider transfer (Now only json)
- [ ] Add xamarin/MAUI, avaloniaui... samples
- [x] Make it base on MS configuration instead of overwrite it(Break change)

# Nuget 

- [Ao.SavableConfig](https://www.nuget.org/packages/Ao.SavableConfig/)
- [Ao.SavableConfig.Binder](https://www.nuget.org/packages/Ao.SavableConfig.Binder/)
- [Ao.SavableConfig.Json](https://www.nuget.org/packages/Ao.SavableConfig.Json/)


## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FCricle%2FAo.SavableConfig.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FCricle%2FAo.SavableConfig?ref=badge_large)