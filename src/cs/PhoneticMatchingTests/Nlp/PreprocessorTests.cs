// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Nlp
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.PhoneticMatching.Nlp.Preprocessor;

    /// <summary>
    /// Tests for the preprocessors.
    /// </summary>
    [TestClass]
    public class PreprocessorTests
    {
        private readonly EnPreProcessor englishPreProcessor = new EnPreProcessor();
        private readonly EnPlacesPreProcessor englishPlacesPreProcessor = new EnPlacesPreProcessor();

        [TestMethod]
        public void GivenStreetAndSaint_ToPlacesProcessor_ExpectProperFormatting()
        {
            var result = this.englishPlacesPreProcessor.PreProcess("St Maurice St");
            Assert.AreEqual("saint maurice street", result, "Place pre-processing doesn't return the expected result.");
        }

        [TestMethod]
        public void GivenCombiningAcuteAndLigature_ToEnglishPreprocessor_ExpectProperFormatting()
        {
            // "Híﬃ"
            // í has a combining acute accent, ﬃ is a ligature
            var result = this.englishPreProcessor.PreProcess("Hi\u0301\uFB03");
            Assert.AreEqual("h\u00EDffi", result);
        }

        [TestMethod]
        public void GivenDigits_ToEnglishPreprocessor_ExpectProperFormatting()
        {
            var result = this.englishPreProcessor.PreProcess("123 King  St");
            Assert.AreEqual("123 king st", result);

            result = this.englishPreProcessor.PreProcess("2 Wildwood  Place");
            Assert.AreEqual("2 wildwood place", result);
        }

        [TestMethod]
        public void GivenPunctuation_ToEnglishPreprocessor_ExpectProperFormatting()
        {
            var result = this.englishPreProcessor.PreProcess("!omg! ch!ll ?how?");
            Assert.AreEqual("omg ch ll how", result);
        }

        [TestMethod]
        public void GivenApostropheAndCase_ToEnglishPreprocessor_ExpectProperFormatting()
        {
            var result = this.englishPreProcessor.PreProcess("Justin's haus");
            Assert.AreEqual("justin s haus", result);
        }

        [TestMethod]
        public void GivenSimpleTokenization_ToEnglishPreprocessor_ExpectProperFormatting()
        {
            var result = this.englishPreProcessor.PreProcess("call mom");
            Assert.AreEqual("call mom", result);

            result = this.englishPreProcessor.PreProcess("call MoM!");
            Assert.AreEqual("call mom", result);

            result = this.englishPreProcessor.PreProcess("*(*&call,   MoM! )_+");
            Assert.AreEqual("call mom", result);

            result = this.englishPreProcessor.PreProcess(":call/mom");
            Assert.AreEqual("call mom", result);

            result = this.englishPreProcessor.PreProcess("Call  mom.");
            Assert.AreEqual("call mom", result);

            result = this.englishPreProcessor.PreProcess("Call  mom .");
            Assert.AreEqual("call mom", result);

            result = this.englishPreProcessor.PreProcess("Call  mom .");
            Assert.AreEqual("call mom", result);
        }
    }
}
