﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Dnx.Compilation
{
    public interface IAnalyzerReference
    {
        string Name { get; }

        IEnumerable<string> Files { get; }
    }
}
