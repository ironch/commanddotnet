﻿using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandDotNet.Logging;

namespace CommandDotNet.Builders
{
    /// <summary>
    /// The version and file information for the application.<br/>
    /// Uses <see cref="Assembly.GetEntryAssembly"/> and determines if app is run via
    /// dotnet console app or as an exe or as a self-contained single executable<br/>
    /// For unit tests, use `AppRunner.Configure(c =&gt; c.Services.Set(new AppInfo(...)))`
    /// to set to specific version
    /// </summary>
    public class AppInfo
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private static AppInfo? sAppInfo;

        internal static AppInfo Instance => sAppInfo ??= BuildAppInfo();

        private string? _version;

        /// <summary>True if the application's filename ends with .exe</summary>
        public bool IsExe { get; }

        /// <summary>True if published as a self-contained single executable</summary>
        public bool IsSelfContainedExe { get; }

        /// <summary>True if run using the dotnet.exe</summary>
        public bool IsRunViaDotNetExe { get; }

        /// <summary>The entry assembly. Could be an exe or dll.</summary>
        public Assembly EntryAssembly { get; }

        /// <summary>The file name used to execute the app</summary>
        public string FilePath { get; }

        /// <summary>The file name used to execute the app</summary>
        public string FileName { get; }

        public string Version => _version ??= GetVersion(Instance.EntryAssembly);

        public AppInfo(
            bool isExe, bool isSelfContainedExe, bool isRunViaDotNetExe, 
            Assembly entryAssembly,
            string filePath, string fileName,
            string? version = null)
        {
            IsExe = isExe;
            IsSelfContainedExe = isSelfContainedExe;
            IsRunViaDotNetExe = isRunViaDotNetExe;
            EntryAssembly = entryAssembly;
            FilePath = filePath;
            FileName = fileName;
            _version = version;
        }

        public static AppInfo GetAppInfo(CommandContext commandContext)
        {
            var svcs = commandContext.AppConfig.Services;
            var appInfo = svcs.GetOrAdd(() => Instance);
            return appInfo;
        }

        private static AppInfo BuildAppInfo()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                throw new AppRunnerException(
                    "Unable to determine version because Assembly.GetEntryAssembly() is null. " +
                    "This is a known issue when running unit tests in .net framework. https://tinyurl.com/y6rnjqsg" +
                    "Set the version info in AppRunner.Configure(c => c.Services.Set(new AppInfo(...))) " +
                    "to create a specific info for tests");
            }

            // this could be dotnet.exe or {app_name}.exe if published as single executable
            var mainModule = Process.GetCurrentProcess().MainModule;
            var mainModuleFilePath = mainModule?.FileName;
            var entryAssemblyFilePath = entryAssembly?.Location;
            
            Log.Debug($"{nameof(mainModuleFilePath)}: {mainModuleFilePath}");
            Log.Debug($"{nameof(entryAssemblyFilePath)}: {entryAssemblyFilePath}");

            var mainModuleFileName = Path.GetFileName(mainModuleFilePath);
            var entryAssemblyFileName = Path.GetFileName(entryAssemblyFilePath);

            // this logic isn't not covered by unit tests.
            // changing this logic requires manual testing of
            // - .dll files run in dotnet
            // - .exe console apps
            // - .dll published as self-contained .exe files.
            // - windows, linux & mac

            var isRunViaDotNetExe = false;
            var isSelfContainedExe = false;
            var isExe = false;
            if (mainModuleFileName != null)
            {
                // osx uses 'dotnet' instead of 'dotnet.exe'
                if (!(isRunViaDotNetExe = mainModuleFileName.Equals("dotnet.exe") || mainModuleFileName.Equals("dotnet")))
                {
                    var entryAssemblyFileNameWithoutExt = Path.GetFileNameWithoutExtension(entryAssemblyFileName);
                    isSelfContainedExe = isExe = mainModuleFileName.EndsWith($"{entryAssemblyFileNameWithoutExt}.exe");
                }
            }

            isExe = isExe || entryAssemblyFileName.EndsWith("exe");

            var filePath = isSelfContainedExe
                ? mainModuleFilePath
                : entryAssemblyFilePath;

            var fileName = Path.GetFileName(filePath);

            Log.Debug($"  {nameof(FileName)}={fileName} " +
                      $"{nameof(IsRunViaDotNetExe)}={isRunViaDotNetExe} " +
                      $"{nameof(IsSelfContainedExe)}={isSelfContainedExe} " +
                      $"{nameof(FilePath)}={filePath}");

            return new AppInfo(isExe, isSelfContainedExe, isRunViaDotNetExe, entryAssembly!, filePath!, fileName);
        }

        private static string GetVersion(Assembly hostAssembly)
        {
            var fvi = FileVersionInfo.GetVersionInfo(hostAssembly.Location);
            return fvi.ProductVersion;
        }
    }
}