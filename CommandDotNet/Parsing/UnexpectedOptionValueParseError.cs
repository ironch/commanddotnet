﻿using System;
using CommandDotNet.Tokens;

namespace CommandDotNet.Parsing
{
    /// <summary>
    /// A value was provided for an option that didn't expect it.
    /// The option has an arity of 1 but was given multiple values.
    /// Cannot be a flag.
    /// </summary>
    public class UnexpectedOptionValueParseError : IParseError
    {
        public string Message { get; }
        public Command Command { get; }
        public Option Option { get; }
        public Token Token { get; }

        public UnexpectedOptionValueParseError(Command command, Option option, Token token)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Option = option ?? throw new ArgumentNullException(nameof(option));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            Message = $"Unexpected value '{token.RawValue}' for option '{option.Name}'";
        }
    }
}