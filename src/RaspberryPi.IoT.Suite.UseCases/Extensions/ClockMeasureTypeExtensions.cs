using System;
using System.Collections.Generic;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Enums;

namespace RaspberryPi.IoT.Suite.UseCases.Extensions
{
    internal static class ClockMeasureTypeExtensions
    {
        private static readonly IReadOnlyDictionary<ClockMeasureType, string> ClockMeasureSubCommandByClockMeasureType =
            new Dictionary<ClockMeasureType, string>
            {
                [ClockMeasureType.Arm] = "arm",
                [ClockMeasureType.Core] = "core",
                [ClockMeasureType.Emmc] = "emmc",
            };

        public static string GetSubCommandFor(this ClockMeasureType clockMeasureType)
        {
            return ClockMeasureSubCommandByClockMeasureType.ContainsKey(clockMeasureType)
                ? ClockMeasureSubCommandByClockMeasureType[clockMeasureType]
                : throw new ArgumentException(
                    $"No sub-command found for {nameof(ClockMeasureType)} {clockMeasureType.ToString()}",
                    nameof(clockMeasureType));
        }
    }
}