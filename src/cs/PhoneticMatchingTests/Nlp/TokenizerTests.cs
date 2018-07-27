// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Nlp
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching.Nlp.Tokenizer;

    [TestClass]
    public class TokenizerTests
    {
        private readonly WhitespaceTokenizer tokenizer = new WhitespaceTokenizer();

        [TestMethod]
        public void GivenEmptyString_ExpectNoToken()
        {
            var result = this.tokenizer.Tokenize(string.Empty);
            Assert.AreEqual(0, result.Count, "Expect no token for empty query");
        }

        [TestMethod]
        public void GivenNoWhitespace_ExpectIdentity()
        {
            const string Query = "example";
            var result = this.tokenizer.Tokenize(Query);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(Query, result[0].Value);
        }

        [TestMethod]
        public void GivenQueryNotEndingWithSpaces_ExpectNoWhitespaceOrEmpty()
        {
            var result = this.tokenizer.Tokenize("  There  are some words, here! #blessed");
            var expected = new string[] { "There", "are", "some", "words,", "here!", "#blessed" };
            this.AssertTokensAreEquals(expected, result);
        }

        [TestMethod]
        public void GivenQueryEndingWithSpaces_ExpectNoWhitespaceOrEmpty()
        {
            var result = this.tokenizer.Tokenize("  There  are some words, here! #blessed  ");
            var expected = new string[] { "There", "are", "some", "words,", "here!", "#blessed" };
            this.AssertTokensAreEquals(expected, result);
        }

        private void AssertTokensAreEquals(string[] expectedValues, IList<Token> tokens)
        {
            Assert.AreEqual(expectedValues.Length, tokens.Count, "Tokenizer didn't return the expected result.");
            for (int idx = 0; idx < expectedValues.Length; ++idx)
            {
                Assert.AreEqual(expectedValues[idx], tokens[idx].Value);
            }
        }
    }
}
