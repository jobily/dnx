﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Dnx.Tooling.Publish;
using Microsoft.Dnx.Runtime;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Dnx.Runtime.Common.CommandLine;

namespace Microsoft.Dnx.Tooling
{
    internal class PublishConsoleCommand
    {
        public static void Register(CommandLineApplication cmdApp, ReportsFactory reportsFactory, IApplicationEnvironment applicationEnvironment)
        {
            cmdApp.Command("publish", c =>
            {
                c.Description = "Publish application for deployment";

                var argProject = c.Argument("[project]", "Path to project, default is current directory");
                var optionOut = c.Option("-o|--out <PATH>", "Where does it go", CommandOptionType.SingleValue);
                var optionConfiguration = c.Option("--configuration <CONFIGURATION>", "The configuration to use for deployment (Debug|Release|{Custom})",
                    CommandOptionType.SingleValue);
                var optionNoSource = c.Option("--no-source", "Compiles the source files into NuGet packages",
                    CommandOptionType.NoValue);
                var optionFramework = c.Option(
                    "--framework",
                    "Name of the frameworks to include.",
                    CommandOptionType.MultipleValue);
                var optionIISCommand = c.Option("--iis-command", "Overrides the command name to use in the web.config for the httpPlatformHandler. The default is web.", CommandOptionType.SingleValue);
                var optionRuntime = c.Option("--runtime <RUNTIME>", "Name or full path of the runtime folder to include, or \"active\" for current runtime on PATH",
                    CommandOptionType.MultipleValue);
                var optionNative = c.Option("--native", "Build and include native images. User must provide targeted CoreCLR runtime versions along with this option.",
                    CommandOptionType.NoValue);
                var optionIncludeSymbols = c.Option("--include-symbols", "Include symbols in output bundle",
                    CommandOptionType.NoValue);
                var optionWwwRoot = c.Option("--wwwroot <NAME>", "Name of public folder in the project directory",
                    CommandOptionType.SingleValue);
                var optionWwwRootOut = c.Option("--wwwroot-out <NAME>",
                    "Name of public folder in the output, can be used only when the '--wwwroot' option or 'webroot' in project.json is specified",
                    CommandOptionType.SingleValue);
                var optionQuiet = c.Option("--quiet", "Do not show output such as source/destination of published files",
                    CommandOptionType.NoValue);
                c.HelpOption("-?|-h|--help");

                c.OnExecute(() =>
                {
                    c.ShowRootCommandFullNameAndVersion();

                    var reports = reportsFactory.CreateReports(optionQuiet.HasValue());

                    // Validate the arguments
                    if (optionFramework.HasValue() && optionRuntime.HasValue())
                    {
                        reports.WriteError($"--{optionFramework.LongName} and --{optionRuntime.LongName} cannot be used together.");
                        return 1;
                    }

                    var options = new PublishOptions
                    {
                        OutputDir = optionOut.Value(),
                        ProjectDir = argProject.Value ?? System.IO.Directory.GetCurrentDirectory(),
                        Configuration = optionConfiguration.Value() ?? "Debug",
                        RuntimeActiveFramework = applicationEnvironment.RuntimeFramework,
                        WwwRoot = optionWwwRoot.Value(),
                        WwwRootOut = optionWwwRootOut.Value() ?? optionWwwRoot.Value(),
                        NoSource = optionNoSource.HasValue(),
                        Runtimes = optionRuntime.HasValue() ?
                            string.Join(";", optionRuntime.Values).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) :
                            new string[0],
                        IISCommand = optionIISCommand.Value(),
                        Native = optionNative.HasValue(),
                        IncludeSymbols = optionIncludeSymbols.HasValue(),
                        Reports = reports
                    };

                    options.AddFrameworkMonikers(optionFramework.Values);

                    var manager = new PublishManager(options, applicationEnvironment);
                    if (!manager.Publish())
                    {
                        return 1;
                    }

                    return 0;
                });
            });
        }
    }
}
