// <copyright file="FontMetaDataParser.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System.IO.Abstractions;
    using System.Linq;

    /// <inheritdoc/>
    internal class FontMetaDataParser : IFontMetaDataParser
    {
        /// <inheritdoc/>
        public FontMetaDataParseResult Parse(string stringToParse)
        {
            const char nameValueSeparator = ':';
            const char metaDataSignifier = '|';
            const bool dataIsValid = true;
            const bool dataIsInvalid = false;
            const uint invalidSize = 0;
            const string emptyMetaData = "";
            const string emptyFilePath = "";
            var doesNotContainMetaData = stringToParse.DoesNotContain(metaDataSignifier);

            // If no metadata exists
            if (doesNotContainMetaData)
            {
                return new FontMetaDataParseResult(
                    !doesNotContainMetaData,
                    dataIsValid,
                    emptyFilePath,
                    emptyMetaData,
                    invalidSize);
            }

            var startsWithMetaDataSignifier = stringToParse.StartsWith(metaDataSignifier);
            var endsWithMetaDataSignifier = stringToParse.EndsWith(metaDataSignifier);
            var tooManyMetaDataSignifiers = stringToParse.Count(c => c == metaDataSignifier) > 1;

            if (startsWithMetaDataSignifier || endsWithMetaDataSignifier || tooManyMetaDataSignifiers)
            {
                return new FontMetaDataParseResult(
                    !doesNotContainMetaData,
                    dataIsInvalid,
                    emptyFilePath,
                    emptyMetaData,
                    invalidSize);
            }

            var doesNotContainNameValueSeparator = stringToParse.DoesNotContain(nameValueSeparator);

            var wholeSections = stringToParse.Split(metaDataSignifier);
            var fullFilePath = wholeSections[0];
            var metaData = wholeSections[1];

            if (doesNotContainNameValueSeparator)
            {
                return new FontMetaDataParseResult(!doesNotContainMetaData, dataIsInvalid, fullFilePath, metaData, invalidSize);
            }

            var startsWithNameValueSeparator = stringToParse.StartsWith(nameValueSeparator);
            var endsWithNameValueSeparator = stringToParse.EndsWith(nameValueSeparator);

            if (startsWithNameValueSeparator || endsWithNameValueSeparator)
            {
                return new FontMetaDataParseResult(
                    !doesNotContainMetaData,
                    dataIsInvalid,
                    fullFilePath,
                    emptyMetaData,
                    invalidSize);
            }

            var metaDataSections = metaData.Split(nameValueSeparator);
            var name = metaDataSections[0];

            if (name.DoesNotOnlyContainsLetters())
            {
                return new FontMetaDataParseResult(
                    !doesNotContainMetaData,
                    dataIsInvalid,
                    fullFilePath,
                    metaData,
                    invalidSize);
            }

            var valueAsString = metaDataSections[1];

            var valueParseSuccess = uint.TryParse(valueAsString, out var valueAsInt);

            return valueParseSuccess
                ? new FontMetaDataParseResult(!doesNotContainMetaData, dataIsValid, fullFilePath, metaData, valueAsInt)
                : new FontMetaDataParseResult(!doesNotContainMetaData, dataIsInvalid, fullFilePath, metaData, invalidSize);
        }
    }
}
