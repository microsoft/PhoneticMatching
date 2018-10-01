// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Preprocessor
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// English Pre-processor with specific rules for places.
    /// </summary>
    public class EnPlacesPreProcessor : EnPreProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnPlacesPreProcessor"/> class.
        /// </summary>
        public EnPlacesPreProcessor()
        {
            // Cardinal Directions
            this.Rules.AddRule(new Regex(@"\be\b"), "east");
            this.Rules.AddRule(new Regex(@"\bn\b"), "north");
            this.Rules.AddRule(new Regex(@"\bs\b"), "south");
            this.Rules.AddRule(new Regex(@"\bw\b"), "west");

            this.Rules.AddRule(new Regex(@"\bne\b"), "north east");
            this.Rules.AddRule(new Regex(@"\bnw\b"), "north west");
            this.Rules.AddRule(new Regex(@"\bse\b"), "south east");
            this.Rules.AddRule(new Regex(@"\bsw\b"), "south west");

            // Address Abbreviations
            // Word boundary doesn't work after the "." so we need look-ahead.
            this.Rules.AddRule(new Regex(@"\baly\.?(?=[\s\p{P}\p{S}]|$)"), "alley");
            this.Rules.AddRule(new Regex(@"\bave?\.?(?=[\s\p{P}\p{S}]|$)"), "avenue");
            this.Rules.AddRule(new Regex(@"\bblvd\.?(?=[\s\p{P}\p{S}]|$)"), "boulevard");
            this.Rules.AddRule(new Regex(@"\bbnd\.?(?=[\s\p{P}\p{S}]|$)"), "bend");
            this.Rules.AddRule(new Regex(@"\bcres\.?(?=[\s\p{P}\p{S}]|$)"), "crescent");
            this.Rules.AddRule(new Regex(@"\bcir\.?(?=[\s\p{P}\p{S}]|$)"), "circle");
            this.Rules.AddRule(new Regex(@"\bct\.?(?=[\s\p{P}\p{S}]|$)"), "court");
            this.Rules.AddRule(new Regex(@"\bdr\.?(?=[\s\p{P}\p{S}]|$)"), "drive");
            this.Rules.AddRule(new Regex(@"\best\.?(?=[\s\p{P}\p{S}]|$)"), "estate");
            this.Rules.AddRule(new Regex(@"\bln\.?(?=[\s\p{P}\p{S}]|$)"), "lane");
            this.Rules.AddRule(new Regex(@"\bpkwy\.?(?=[\s\p{P}\p{S}]|$)"), "parkway");
            this.Rules.AddRule(new Regex(@"\bpl\.?(?=[\s\p{P}\p{S}]|$)"), "place");
            this.Rules.AddRule(new Regex(@"\brd\.?(?=[\s\p{P}\p{S}]|$)"), "road");

            // Assume "st" at the beginning is for "saint".
            this.Rules.AddRule(new Regex(@"^st\.?(?=[\s\p{P}\p{S}]|$)"), "saint");

            // If "st" does not occur at the start of the string, then we cannot known if it is for "saint" or "street".
            this.Rules.AddRule(new Regex(@"\bst\.?(?=[\s\p{P}\p{S}]|$)"), "street");
            this.Rules.AddRule(new Regex(@"\bxing\.?(?=[\s\p{P}\p{S}]|$)"), "crossing");
        }
    }
}
