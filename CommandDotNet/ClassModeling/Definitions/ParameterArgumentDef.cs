﻿using System;
using System.Reflection;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class ParameterArgumentDef : IArgumentDef
    {
        private readonly ParameterInfo _parameterInfo;
        private readonly Action<object> _assignValue;

        public CommandNodeType CommandNodeType { get; }

        public string Name { get; }

        public string SourcePath => _parameterInfo.FullName(includeNamespace: true);

        public Type Type => _parameterInfo.ParameterType;

        public bool HasDefaultValue => _parameterInfo.HasDefaultValue;

        public object DefaultValue => _parameterInfo.DefaultValue;

        public IArgument Argument { get; set; }

        public ICustomAttributeProvider CustomAttributes => _parameterInfo;

        public ParameterArgumentDef(
            ParameterInfo parameterInfo,
            CommandNodeType commandNodeType,
            AppConfig appConfig,
            Action<object> assignValue)
        {
            _parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));
            _assignValue = assignValue ?? throw new ArgumentNullException(nameof(assignValue));

            CommandNodeType = commandNodeType;

            Name = parameterInfo.BuildName(commandNodeType, appConfig);
        }

        public void SetValue(object value)
        {
            _assignValue(value);
        }

        public override string ToString()
        {
            return $"Parameter:{SourcePath} > {Name}({Type})";
        }
    }
}