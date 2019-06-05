using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.BddTests.Models
{
    public interface IArgsDefaultsSampleTypesMethod
    {
        void ArgsDefaults(
            string stringArg = "lala",
            int structArg = 3,
            int? structNArg = 4,
            DayOfWeek enumArg = DayOfWeek.Wednesday,
            Uri objectArg = null,
            List<string> stringListArg = null);

        void StructListDefaults(List<int> structListArg = null);
        void EnumListDefaults(List<DayOfWeek> enumListArg = null);
        void ObjectListDefaults(List<Uri> objectListArg = null);
    }
}