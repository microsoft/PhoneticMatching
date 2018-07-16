// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/pronunciation.hpp"
#include "maluuba/xtd/string_view.hpp"
#include <exception>
#include <stdexcept>
#include <string>
#include <unordered_map>

namespace maluuba
{
namespace speech
{
  namespace
  {
    /** Map from Arpabet phonemes to IPA pronunciations. */
    const std::u16string&
    arpabet_to_ipa(const xtd::string_view phoneme)
    {
      static std::unordered_map<xtd::string_view, std::u16string> arpabet_map = {
        // Vowels

        // Monophthongs
        {"AO", u"ɔ"},
        {"AA", u"ɑ"},
        {"IY", u"i"},
        {"UW", u"u"},
        {"EH", u"ɛ"},
        {"IH", u"ɪ"},
        {"UH", u"ʊ"},
        {"AH", u"ʌ"},
        {"AX", u"ə"},
        {"AE", u"æ"},

        // Diphthongs
        {"EY", u"eɪ̯"},
        {"AY", u"aɪ̯"},
        {"OW", u"oʊ̯"},
        {"AW", u"aʊ̯"},
        {"OY", u"ɔɪ̯"},

        // Rhotic
        {"ER",  u"ɝ"},
        {"AXR", u"ɚ"},

        // Consonants

        // Stops
        {"P", u"p"},
        {"B", u"b"},
        {"T", u"t"},
        {"D", u"d"},
        {"K", u"k"},
        {"G", u"ɡ"},

        // Affricates
        {"CH", u"tʃ"},
        {"JH", u"dʒ"},

        // Fricatives
        {"F",  u"f"},
        {"V",  u"v"},
        {"TH", u"θ"},
        {"DH", u"ð"},
        {"S",  u"s"},
        {"Z",  u"z"},
        {"SH", u"ʃ"},
        {"ZH", u"ʒ"},
        {"HH", u"h"},

        // Nasals
        {"M",   u"m"},
        {"EM",  u"m̩"},
        {"N",   u"n"},
        {"EN",  u"n̩"},
        {"NG",  u"ŋ"},
        {"ENG", u"ŋ̍"},

        // Liquids
        {"L",  u"lˠ"},
        {"EL", u"l̩ˠ"},
        {"R",  u"r"},
        {"DX", u"ɾ"},
        {"NX", u"ɾ̃"},

        // Semivowels
        {"Y", u"j"},
        {"W", u"w"},
        {"Q", u"ʔ"},

        // Suprasegmentals
        {" ", u" "},
      };

      auto found = arpabet_map.find(phoneme);
      if (found == arpabet_map.end()) {
        throw std::domain_error("Unrecognized ARPABET phoneme `" + std::string{phoneme} + "`.");
      }
      return found->second;
    }
  }

  EnPronunciation
  EnPronunciation::from_arpabet(const std::vector<std::string>& arpabet)
  {
    std::u16string ipa;

    for (const auto& phoneme : arpabet) {
      std::string copy{phoneme.begin(), phoneme.end()};

      // Convert to uppercase
      for (auto& c : copy) {
        if (c >= 'a' && c <= 'z') {
          c += 'A' - 'a';
        }
      }

      if (!copy.empty()) {
        auto last = copy[copy.length() - 1];
        if (last >= '0' && last <= '2') {
          copy.resize(copy.length() - 1);
        }
      }

      ipa += arpabet_to_ipa(copy);
    }

    return {std::move(ipa)};
  }
}
}
