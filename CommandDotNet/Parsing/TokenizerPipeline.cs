﻿using System;
using System.Linq;

namespace CommandDotNet.Parsing
{
    internal static class TokenizerPipeline
    {
        public static int Tokenize(ExecutionResult executionResult, Func<ExecutionResult, int> next)
        {
            InsertSystemTransformations(executionResult.ParserConfig);
            executionResult.Tokens = ApplyInputTransformations(executionResult.Tokens, executionResult.ParserConfig);
            executionResult.ParserConfig.Events.TokenizationCompleted();

            return next(executionResult);
        }

        private static TokenCollection ApplyInputTransformations(TokenCollection tokens, ParserConfig parserConfig)
        {
            foreach (var transformation in parserConfig.InputTransformations)
            {
                try
                {
                    var tempArgs = transformation.Transformation(tokens);
                    parserConfig.Events.InputTransformation(transformation, tokens, tempArgs);
                    tokens = tempArgs;
                }
                catch (Exception e)
                {
                    throw new AppRunnerException($"transformation failure for: {transformation}", e);
                }
            }

            return tokens;
        }

        private static void InsertSystemTransformations(ParserConfig parserConfig)
        {
            parserConfig.InputTransformations = parserConfig.InputTransformations.OrderBy(t => t.Order).AsEnumerable();
            // append system transformations to the end.
            // these are features we want to ensure are applied to all arguments
            // because parsing logic depends on these being processed
            parserConfig.InputTransformations = parserConfig.InputTransformations.Union(
                new[]
                {
                    new InputTransformation(
                        "expand-clubbed-flags",
                        Int32.MaxValue,
                        Tokenizer.ExpandClubbedOptions),
                    new InputTransformation(
                        "split-option-assignments",
                        Int32.MaxValue,
                        Tokenizer.SplitOptionAssignments)
                });
        }
    }
}