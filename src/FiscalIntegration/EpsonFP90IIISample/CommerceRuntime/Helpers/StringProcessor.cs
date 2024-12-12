/**
* SAMPLE CODE NOTICE
* 
* THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
* OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
* THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
* NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Contoso
{
    namespace CommerceRuntime.DocumentProvider.EpsonFP90IIISample.Helpers
    {
        using System.Collections.Generic;
        using System.Linq;
        using Microsoft.Dynamics.Commerce.Runtime;
        using System;

        /// <summary>
        /// The hepler class is used to process string.
        /// </summary>
        public static class StringProcessor
        {
            /// <summary>
            /// Splits string by words that the string length is more than the maximum length.
            /// </summary>
            /// <param name="inputString">The input string.</param>
            /// <param name="maxLength">The max length the splited string should be.</param>
            /// <returns>The list of splited string.</returns>
            public static List<string> SplitStringByWords(string inputString, int maxLength)
            {
                if (string.IsNullOrWhiteSpace(inputString)) return new List<string>();

                // Split string by words, if the length of word is longer than max length, then splits the word by max length.
                IEnumerable<string> splitWords = inputString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .SelectMany(word => SplitWordByLength(word, maxLength));

                IEnumerable<string> splitLines = MergeSplitWordsToLines(splitWords, maxLength);

                return splitLines.ToList();
            }

            /// <summary>
            /// Merges splited words to lines by max length.
            /// </summary>
            /// <param name="inputStrings">The input strings.</param>
            /// <param name="maxLength">The max length.</param>
            /// <returns>An IEnumerable of merged string.</returns>
            private static IEnumerable<string> MergeSplitWordsToLines(IEnumerable<string> inputStrings, int maxLength)
            {
                ThrowIf.Null(inputStrings, nameof(inputStrings));

                string currentLine = string.Empty;
                string separator = " ";

                foreach (string str in inputStrings)
                {
                    if (currentLine.Length == 0)
                    {
                        currentLine = str;
                    }
                    else if (currentLine.Length + separator.Length + str.Length > maxLength)
                    {
                        yield return currentLine;
                        currentLine = str;
                    }
                    else
                    {
                        currentLine += separator + str;
                    }
                }

                if (currentLine.Length != 0)
                {
                    yield return currentLine;
                }
            }

            /// <summary>
            /// Splits word by max length, if the length of word is longer than max length, then splits by max length.
            /// </summary>
            /// <param name="str">The word needs to be splited.</param>
            /// <param name="maxLength">The max length.</param>
            /// <returns>An IEnumerable of splited word.</returns>
            private static IEnumerable<string> SplitWordByLength(string word, int maxLength)
            {
                for (int i = 0; i < word.Length; i += maxLength)
                    yield return word.Substring(i, Math.Min(maxLength, word.Length - i));
            }
        }
    }
}
