﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Microsoft.Dnx.Testing
{
    public static class ProjectExtensions
    {
        public static string GetBinPath(this Runtime.Project project)
        {
            return Path.Combine(project.ProjectDirectory, "bin");
        }

        public static string GetLocalPackagesDir(this Runtime.Project project)
        {
            return Path.Combine(project.ProjectDirectory, "packages");
        }

        public static void UpdateProjectFile(this Runtime.Project project, Action<JObject> updateContents)
        {
            JsonUtils.UpdateJson(project.ProjectFilePath, updateContents);
        }
    }
}
