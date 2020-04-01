# Help

Help documentation is automatically generated for the application.

!!! Tip
    To change the application name in the usage section, see [UsageAppName](#usageappname) and [UsageAppNameStyle](#usageappnamestyle)

## Default Template

The default template is as follows

```bash
{description}

Usage: {application} [command] [options] [arguments]

Arguments:

  {name}  (Multiple) <TYPE> [default] 
  {description}
  {allowed-values}

Options:

  -{short} | --{long} (Multiple) <TYPE> [default] 
  {description}
  {allowed-values}

Options also available for subcommands:

  -{short} | --{long} (Multiple) <TYPE> [default] 
  {description}
  {allowed-values}

Commands:

  {name} {description}

{extended-help-text}

Use "{application} [command] --help" for more information about a command.
```

Segments that do not apply for a command are not included.  

For example, the last line are only included when a command has subcommands
and "Options also available for subcommands:" is only included when options 
defined in parent commands can be provided in subcommands.

## Modify behavior via settings

There are a few settings to alter the default behavior, accessed via `AppSettings.Help`

```c#
new AppSettings
{
    Help
    {
        PrintHelpOption = true,
        ExpandArgumentsInUsage = true,
        TextStyle = HelpTextStyle.Basic,
        UsageAppNameStyle = UsageAppNameStyle.Executable,
        UsageAppName = "GlobalToolName"
    }
}
```

### PrintHelpOption
`PrintHelpOption` will include the help option in the list of options for every command.

### ExpandArgumentsInUsage
When true, arguments are expanded in the usage section so the names of all arguments are shown.

Given the command: `Add(int value1, int value2, int value3 = 0)`

Usage is `Add [arguments]` 

When ExpandArgumentsInUsage = true

Usage is `Add <value1> <value2> [<value3>]`


### TextStyle
Default is `HelpTextStyle.Detailed`. `HelpTextStyle.Basic` changes the argument and option template to just `{name} {description}`

### UsageAppNameStyle

This determines what is used for {application} in `Usage: {application}`. 

When `Executable`, the name will be `Usage: {filename}`.

When `DotNet`, the name will be `Usage: dotnet {filename}`

When `Adaptive`, if the file name ends with ".exe", then uses `Executable` else `DotNet`.

`Adaptive` & `Executable` will also detect when the app is a [self-contained executable](https://docs.microsoft.com/en-us/dotnet/core/deploying/#produce-an-executable) and use the executable filename.

### UsageAppName

When specified, this value will be used and `UsageAppNameStyle` will be ignored.

### %UsageAppName% Tempate

When you want to show usage examples in a command description, extended help or overridden usage section, use `%UsageAppName%`. This text will be replaced usage app name from one of the options above.  

See this line `"Example: %UsageAppName% [debug] [parse] [log:info] cancel-me"` in the [Example app](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet.Example/Examples.cs#L14).

## Custom Help Provider

The template is driven by the [HelpTextProvider](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Help/HelpTextProvider.cs) class.

There are two options for overriding the behavior. 

The first and simplest option is to inherit from `HelpTextProvider` and override the method for the section to modify.

The second option is to implement a new [IHelpProvider](https://github.com/bilal-fazlani/commanddotnet/blob/master/CommandDotNet/Help/IHelpProvider.cs).

Configure the provider with: `appRunner.Configure(b => b.CustomHelpProvider = new MyHelpProvider());`