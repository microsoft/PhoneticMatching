// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/pronunciation/impl.hpp"
#include "maluuba/speech/pronunciation.hpp"
#include "maluuba/debug.hpp"
#include "maluuba/unicode.hpp"
#include <cstdint>
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
    // To make the letter table easier to read

    constexpr auto VOICELESS = Phonation::VOICELESS;
    constexpr auto VOICED    = Phonation::MODAL;

    constexpr auto BILABIAL        = PlaceOfArticulation::BILABIAL;
    constexpr auto LABIODENTAL     = PlaceOfArticulation::LABIODENTAL;
    constexpr auto DENTAL          = PlaceOfArticulation::DENTAL;
    constexpr auto ALVEOLAR        = PlaceOfArticulation::ALVEOLAR;
    constexpr auto PALATO_ALVEOLAR = PlaceOfArticulation::PALATO_ALVEOLAR;
    constexpr auto RETROFLEX       = PlaceOfArticulation::RETROFLEX;
    constexpr auto ALVEOLO_PALATAL = PlaceOfArticulation::ALVEOLO_PALATAL;
    constexpr auto LABIAL_PALATAL  = PlaceOfArticulation::LABIAL_PALATAL;
    constexpr auto PALATAL         = PlaceOfArticulation::PALATAL;
    constexpr auto PALATAL_VELAR   = PlaceOfArticulation::PALATAL_VELAR;
    constexpr auto LABIAL_VELAR    = PlaceOfArticulation::LABIAL_VELAR;
    constexpr auto VELAR           = PlaceOfArticulation::VELAR;
    constexpr auto UVULAR          = PlaceOfArticulation::UVULAR;
    constexpr auto PHARYNGEAL      = PlaceOfArticulation::PHARYNGEAL;
    constexpr auto EPIGLOTTAL      = PlaceOfArticulation::EPIGLOTTAL;
    constexpr auto GLOTTAL         = PlaceOfArticulation::GLOTTAL;

    constexpr auto NASAL                  = MannerOfArticulation::NASAL;
    constexpr auto PLOSIVE                = MannerOfArticulation::PLOSIVE;
    constexpr auto SIBILANT_FRICATIVE     = MannerOfArticulation::SIBILANT_FRICATIVE;
    constexpr auto NON_SIBILANT_FRICATIVE = MannerOfArticulation::NON_SIBILANT_FRICATIVE;
    constexpr auto APPROXIMANT            = MannerOfArticulation::APPROXIMANT;
    constexpr auto FLAP                   = MannerOfArticulation::FLAP;
    constexpr auto TRILL                  = MannerOfArticulation::TRILL;
    constexpr auto LATERAL_FRICATIVE      = MannerOfArticulation::LATERAL_FRICATIVE;
    constexpr auto LATERAL_APPROXIMANT    = MannerOfArticulation::LATERAL_APPROXIMANT;
    constexpr auto LATERAL_FLAP           = MannerOfArticulation::LATERAL_FLAP;
    constexpr auto CLICK                  = MannerOfArticulation::CLICK;
    constexpr auto IMPLOSIVE              = MannerOfArticulation::IMPLOSIVE;

    constexpr auto CLOSE      = VowelHeight::CLOSE;
    constexpr auto NEAR_CLOSE = VowelHeight::NEAR_CLOSE;
    constexpr auto CLOSE_MID  = VowelHeight::CLOSE_MID;
    constexpr auto MID        = VowelHeight::MID;
    constexpr auto OPEN_MID   = VowelHeight::OPEN_MID;
    constexpr auto NEAR_OPEN  = VowelHeight::NEAR_OPEN;
    constexpr auto OPEN       = VowelHeight::OPEN;

    constexpr auto FRONT      = VowelBackness::FRONT;
    constexpr auto NEAR_FRONT = VowelBackness::NEAR_FRONT;
    constexpr auto CENTRAL    = VowelBackness::CENTRAL;
    constexpr auto NEAR_BACK  = VowelBackness::NEAR_BACK;
    constexpr auto BACK       = VowelBackness::BACK;

    constexpr auto UNROUNDED = VowelRoundedness::UNROUNDED;
    constexpr auto ROUNDED   = VowelRoundedness::ROUNDED;

    using internal::consonant;
    using internal::vowel;

    const std::uint16_t*
    ipa_letter_repr(char16_t c)
    {
      static std::unordered_map<char16_t, std::uint16_t> ipa_map = {
        // Pulmonic consonants

        // Bilabial
        {u'p', consonant(VOICELESS, BILABIAL, PLOSIVE)},
        {u'b', consonant(VOICED,    BILABIAL, PLOSIVE)},
        {u'm', consonant(VOICED,    BILABIAL, NASAL)},
        {u'ʙ', consonant(VOICED,    BILABIAL, TRILL)},
        {u'ɸ', consonant(VOICELESS, BILABIAL, NON_SIBILANT_FRICATIVE)},
        {u'β', consonant(VOICED,    BILABIAL, NON_SIBILANT_FRICATIVE)},

        // Labiodental
        {u'ɱ', consonant(VOICED,    LABIODENTAL, NASAL)},
        {u'ⱱ', consonant(VOICED,    LABIODENTAL, FLAP)},
        {u'f', consonant(VOICELESS, LABIODENTAL, NON_SIBILANT_FRICATIVE)},
        {u'v', consonant(VOICED,    LABIODENTAL, NON_SIBILANT_FRICATIVE)},
        {u'ʋ', consonant(VOICED,    LABIODENTAL, APPROXIMANT)},

        // Dental
        {u'θ', consonant(VOICELESS, DENTAL, NON_SIBILANT_FRICATIVE)},
        {u'ð', consonant(VOICED,    DENTAL, NON_SIBILANT_FRICATIVE)},

        // Alveolar
        {u't', consonant(VOICELESS, ALVEOLAR, PLOSIVE)},
        {u'd', consonant(VOICED,    ALVEOLAR, PLOSIVE)},
        {u'n', consonant(VOICED,    ALVEOLAR, NASAL)},
        {u'r', consonant(VOICED,    ALVEOLAR, TRILL)},
        {u'ɾ', consonant(VOICED,    ALVEOLAR, FLAP)},
        {u'ɺ', consonant(VOICED,    ALVEOLAR, LATERAL_FLAP)},
        {u's', consonant(VOICELESS, ALVEOLAR, SIBILANT_FRICATIVE)},
        {u'z', consonant(VOICED,    ALVEOLAR, SIBILANT_FRICATIVE)},
        {u'ɹ', consonant(VOICED,    ALVEOLAR, APPROXIMANT)},
        {u'ɬ', consonant(VOICELESS, ALVEOLAR, LATERAL_FRICATIVE)},
        {u'ɮ', consonant(VOICED,    ALVEOLAR, LATERAL_FRICATIVE)},
        {u'l', consonant(VOICED,    ALVEOLAR, LATERAL_APPROXIMANT)},

        // Palato-alveolar
        {u'ʃ', consonant(VOICELESS, PALATO_ALVEOLAR, SIBILANT_FRICATIVE)},
        {u'ʒ', consonant(VOICED,    PALATO_ALVEOLAR, SIBILANT_FRICATIVE)},

        // Retroflex
        {u'ʈ', consonant(VOICELESS, RETROFLEX, PLOSIVE)},
        {u'ɖ', consonant(VOICED,    RETROFLEX, PLOSIVE)},
        {u'ɳ', consonant(VOICED,    RETROFLEX, NASAL)},
        {u'ɽ', consonant(VOICED,    RETROFLEX, FLAP)},
        {u'ʂ', consonant(VOICELESS, RETROFLEX, SIBILANT_FRICATIVE)},
        {u'ʐ', consonant(VOICED,    RETROFLEX, SIBILANT_FRICATIVE)},
        {u'ɻ', consonant(VOICED,    RETROFLEX, APPROXIMANT)},
        {u'ɭ', consonant(VOICED,    RETROFLEX, LATERAL_APPROXIMANT)},

        // Alveolo-palatal
        {u'ɕ', consonant(VOICELESS, ALVEOLO_PALATAL, SIBILANT_FRICATIVE)},
        {u'ʑ', consonant(VOICED,    ALVEOLO_PALATAL, SIBILANT_FRICATIVE)},

        // Labial-palatal
        {u'ɥ', consonant(VOICED, LABIAL_PALATAL, APPROXIMANT)},

        // Palatal
        {u'c', consonant(VOICELESS, PALATAL, PLOSIVE)},
        {u'ɟ', consonant(VOICED,    PALATAL, PLOSIVE)},
        {u'ɲ', consonant(VOICED,    PALATAL, NASAL)},
        {u'ç', consonant(VOICELESS, PALATAL, NON_SIBILANT_FRICATIVE)},
        {u'ʝ', consonant(VOICED,    PALATAL, NON_SIBILANT_FRICATIVE)},
        {u'j', consonant(VOICED,    PALATAL, APPROXIMANT)},
        {u'ʎ', consonant(VOICED,    PALATAL, LATERAL_APPROXIMANT)},

        // Palatal-velar
        {u'ɧ', consonant(VOICELESS, PALATAL_VELAR, NON_SIBILANT_FRICATIVE)},

        // Labial-velar
        {u'ʍ', consonant(VOICELESS, LABIAL_VELAR, APPROXIMANT)},
        {u'w', consonant(VOICED,    LABIAL_VELAR, APPROXIMANT)},

        // Velar
        {u'k', consonant(VOICELESS, VELAR, PLOSIVE)},
        {u'ɡ', consonant(VOICED,    VELAR, PLOSIVE)},
        {u'ŋ', consonant(VOICED,    VELAR, NASAL)},
        {u'x', consonant(VOICELESS, VELAR, NON_SIBILANT_FRICATIVE)},
        {u'ɣ', consonant(VOICED,    VELAR, NON_SIBILANT_FRICATIVE)},
        {u'ɰ', consonant(VOICED,    VELAR, APPROXIMANT)},
        {u'ʟ', consonant(VOICED,    VELAR, LATERAL_APPROXIMANT)},

        // Uvular
        {u'q', consonant(VOICELESS, UVULAR, PLOSIVE)},
        {u'ɢ', consonant(VOICED,    UVULAR, PLOSIVE)},
        {u'ɴ', consonant(VOICED,    UVULAR, NASAL)},
        {u'ʀ', consonant(VOICED,    UVULAR, TRILL)},
        {u'χ', consonant(VOICELESS, UVULAR, NON_SIBILANT_FRICATIVE)},
        {u'ʁ', consonant(VOICED,    UVULAR, NON_SIBILANT_FRICATIVE)},

        // Pharyngeal
        {u'ħ', consonant(VOICELESS, PHARYNGEAL, NON_SIBILANT_FRICATIVE)},
        {u'ʕ', consonant(VOICED,    PHARYNGEAL, NON_SIBILANT_FRICATIVE)},

        // Epiglottal
        {u'ʡ', consonant(VOICED,    EPIGLOTTAL, PLOSIVE)},
        {u'ʜ', consonant(VOICELESS, EPIGLOTTAL, NON_SIBILANT_FRICATIVE)},
        {u'ʢ', consonant(VOICED,    EPIGLOTTAL, NON_SIBILANT_FRICATIVE)},

        // Glottal
        {u'ʔ', consonant(VOICELESS, GLOTTAL, PLOSIVE)},
        {u'h', consonant(VOICELESS, GLOTTAL, NON_SIBILANT_FRICATIVE)},
        {u'ɦ', consonant(VOICED,    GLOTTAL, NON_SIBILANT_FRICATIVE)},

        // Non-pulmonic consonants
        {u'ʘ', consonant(VOICELESS, BILABIAL, CLICK)},
        {u'ǀ', consonant(VOICELESS, DENTAL,   CLICK)},
        {u'ǃ', consonant(VOICELESS, ALVEOLAR, CLICK)},
        {u'ǂ', consonant(VOICELESS, PALATAL,  CLICK)},
        {u'ǁ', consonant(VOICELESS, ALVEOLAR, CLICK)},
        {u'ɓ', consonant(VOICED,    BILABIAL, IMPLOSIVE)},
        {u'ɗ', consonant(VOICED,    ALVEOLAR, IMPLOSIVE)},
        {u'ʄ', consonant(VOICED,    PALATAL,  IMPLOSIVE)},
        {u'ɠ', consonant(VOICED,    VELAR,    IMPLOSIVE)},
        {u'ʛ', consonant(VOICED,    UVULAR,   IMPLOSIVE)},

        // Vowels

        // Front
        {u'i', vowel(CLOSE,     FRONT, UNROUNDED)},
        {u'y', vowel(CLOSE,     FRONT, ROUNDED)},
        {u'e', vowel(CLOSE_MID, FRONT, UNROUNDED)},
        {u'ø', vowel(CLOSE_MID, FRONT, ROUNDED)},
        {u'ɛ', vowel(OPEN_MID,  FRONT, UNROUNDED)},
        {u'œ', vowel(OPEN_MID,  FRONT, ROUNDED)},
        {u'æ', vowel(NEAR_OPEN, FRONT, UNROUNDED)},
        {u'a', vowel(OPEN,      FRONT, UNROUNDED)},
        {u'ɶ', vowel(OPEN,      FRONT, ROUNDED)},

        // Near-front
        {u'ɪ', vowel(NEAR_CLOSE, NEAR_FRONT, UNROUNDED)},
        {u'ʏ', vowel(NEAR_CLOSE, NEAR_FRONT, ROUNDED)},

        // Central
        {u'ɨ', vowel(CLOSE,     CENTRAL, UNROUNDED)},
        {u'ʉ', vowel(CLOSE,     CENTRAL, ROUNDED)},
        {u'ɘ', vowel(CLOSE_MID, CENTRAL, UNROUNDED)},
        {u'ɵ', vowel(CLOSE_MID, CENTRAL, ROUNDED)},
        {u'ə', vowel(MID,       CENTRAL, UNROUNDED)},
        {u'ɜ', vowel(OPEN_MID,  CENTRAL, UNROUNDED)},
        {u'ɞ', vowel(OPEN_MID,  CENTRAL, ROUNDED)},
        {u'ɐ', vowel(NEAR_OPEN, CENTRAL, UNROUNDED)},

        // Central rhotic
        {u'ɚ', vowel(MID,       CENTRAL, UNROUNDED, true)},
        {u'ɝ', vowel(OPEN_MID,  CENTRAL, UNROUNDED, true)},

        // Near-back
        {u'ʊ', vowel(NEAR_CLOSE, NEAR_BACK, ROUNDED)},

        // Back
        {u'ɯ', vowel(CLOSE,     BACK, UNROUNDED)},
        {u'u', vowel(CLOSE,     BACK, ROUNDED)},
        {u'ɤ', vowel(CLOSE_MID, BACK, UNROUNDED)},
        {u'o', vowel(CLOSE_MID, BACK, ROUNDED)},
        {u'ʌ', vowel(OPEN_MID,  BACK, UNROUNDED)},
        {u'ɔ', vowel(OPEN_MID,  BACK, ROUNDED)},
        {u'ɑ', vowel(OPEN,      BACK, UNROUNDED)},
        {u'ɒ', vowel(OPEN,      BACK, ROUNDED)},
      };

      auto found = ipa_map.find(c);
      if (found == ipa_map.end()) {
        return nullptr;
      } else {
        return &found->second;
      }
    }
  }

  Pronunciation::Pronunciation(std::u16string ipa)
  {
    for (auto c : ipa) {
      auto repr = ipa_letter_repr(c);
      if (repr) {
        m_phones.push_back(Phone{*repr});
      } else if (m_phones.empty()) {
        std::u16string cstr;
        cstr += c;
        throw std::invalid_argument("Unexpected `" + unicode_cast<std::string>(cstr) + "`.");
      } else {
        auto& phone = m_phones.back();

        switch (c) {
          case u'\u0329': // Syllabic (under)
          case u'\u030D': // Syllabic (over)
            phone.syllabic(true);
            break;

          case u'\u032F': // Non-syllabic
            phone.syllabic(false);
            break;

          case u'\u0325': // Voiceless (under)
          case u'\u030A': // Voiceless (over)
            if (phone.phonation() != Phonation::VOICELESS) {
              // IPA has no diacritic for slack voice, so a voiced consonant
              // with a voiceless diacritic means slack
              phone.phonation(Phonation::SLACK);
            }
            break;

          case u'\u032C': // Voiced
            if (phone.phonation() == Phonation::VOICELESS) {
              phone.phonation(Phonation::MODAL);
            } else {
              // IPA has no diacritic for stiff voice, so an already voiced
              // consonant with a voiced diacritic means stiff
              phone.phonation(Phonation::STIFF);
            }
            break;

          case u'\u0324': // Breathy voiced
            phone.phonation(Phonation::BREATHY);
            break;

          case u'\u0330': // Creaky voiced
            phone.phonation(Phonation::CREAKY);
            break;

          case u'\u0339': // More rounded
            switch (phone.roundedness()) {
              case VowelRoundedness::UNROUNDED:
                phone.roundedness(VowelRoundedness::LESS_ROUNDED);
                break;

              case VowelRoundedness::LESS_ROUNDED:
                phone.roundedness(VowelRoundedness::ROUNDED);
                break;

              case VowelRoundedness::ROUNDED:
              case VowelRoundedness::MORE_ROUNDED:
                phone.roundedness(VowelRoundedness::MORE_ROUNDED);
                break;
            }
            break;

          case u'\u031C': // Less rounded
            switch (phone.roundedness()) {
              case VowelRoundedness::UNROUNDED:
              case VowelRoundedness::LESS_ROUNDED:
                phone.roundedness(VowelRoundedness::UNROUNDED);
                break;

              case VowelRoundedness::ROUNDED:
                phone.roundedness(VowelRoundedness::LESS_ROUNDED);
                break;

              case VowelRoundedness::MORE_ROUNDED:
                phone.roundedness(VowelRoundedness::ROUNDED);
                break;
            }
            break;

          case u'\u02DE': // Rhotacized
            phone.rhotic(true);
            break;

          // TODO: Remaining diacritics
          default:
            // Skip unknown diacritic
            continue;
        }
      }

      m_ipa += c;
    }
  }

  EnPronunciation
  EnPronunciation::from_ipa(const xtd::string_view ipa)
  {
    return {unicode_cast<std::u16string>(ipa)};
  }

  EnPronunciation
  EnPronunciation::subrange(iterator first, iterator last) const
  {
    // We do a linear scan to align the phones with the IPA representation.
    // TODO: Evaluate the memory impact of storing the alignment explicitly.

    auto ipa_first = m_ipa.begin();
    for (auto i = begin(); i != first; ++i) {
      do {
        ++ipa_first;
      } while (ipa_first != m_ipa.end() && !ipa_letter_repr(*ipa_first));
    }

    auto ipa_last = ipa_first;
    for (auto i = first; i != last; ++i) {
      do {
        ++ipa_last;
      } while (ipa_last != m_ipa.end() && !ipa_letter_repr(*ipa_last));
    }

    auto offset = ipa_first - m_ipa.begin();
    auto length = ipa_last - ipa_first;

    EnPronunciation result;
    result.m_ipa = m_ipa.substr(offset, length);
    result.m_phones.assign(first, last);
    return result;
  }
}
}
