﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace git_diff_xlsx.ElementHandlers
{
    public class CellEndElementHandler : EndElementHandler
    {
        private readonly string[] _sharedStringTable;
        private readonly Dictionary<int, string> _numberingFormatsByStyleIndex;

        public CellEndElementHandler(string[] sharedStringTable,  Dictionary<int, string> numberingFormatsByStyleIndex)
        {
            _sharedStringTable = sharedStringTable;
            _numberingFormatsByStyleIndex = numberingFormatsByStyleIndex;
        }

        public override void Invoke(CellContext cellContext, TextWriter output)
        {
            if (cellContext != null)
            {
                output.Write($"    {Format(cellContext)}\n");
            }
        }

        public string Format(CellContext cellContext)
        {
            var value = cellContext.GetValue();
            if (cellContext.ValueType == CellValueTypeEnum.LookupString)
            {
                value = _sharedStringTable[int.Parse(value)];
            }

            if(!string.IsNullOrEmpty(value) && cellContext.StyleIndex.HasValue && _numberingFormatsByStyleIndex.TryGetValue(cellContext.StyleIndex.Value, out var numberingFormat))
            {
                if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var d))
                    value = d.ToString(numberingFormat.Replace('_', ' '), CultureInfo.InvariantCulture);
            }

            return string.IsNullOrEmpty(value) && string.IsNullOrEmpty(cellContext.Formula)
                ? null
                : $"{cellContext.Address}: {value} {cellContext.Formula}".Trim();
        }
    }
}
