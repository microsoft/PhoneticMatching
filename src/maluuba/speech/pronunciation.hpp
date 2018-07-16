/**
 * @file
 * Types for describing and processing prounciation and speech.
 *
 * @author Benedicte Pierrejean
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

// See https://www.internationalphoneticassociation.org/
// and http://en.wikipedia.org/wiki/International_Phonetic_Alphabet

#ifndef MALUUBA_SPEECH_PRONUNCIATION_HPP
#define MALUUBA_SPEECH_PRONUNCIATION_HPP

#include "maluuba/xtd/string_view.hpp"
#include <cstdint>
#include <iosfwd>
#include <string>
#include <vector>

namespace maluuba
{
namespace speech
{
  /**
   * Phone type (consonant or vowel).
   */
  enum class PhoneType
  {
    CONSONANT,
    VOWEL,
  };

  /**
   * Phonation (voice intensity).
   */
  enum class Phonation
  {
    VOICELESS,
    BREATHY,
    SLACK,
    MODAL,
    STIFF,
    CREAKY,
    GLOTTAL_CLOSURE,
  };

  /**
   * Place of articulation for consonants.
   */
  enum class PlaceOfArticulation
  {
    BILABIAL,
    LABIODENTAL,
    DENTAL,
    ALVEOLAR,
    PALATO_ALVEOLAR,
    RETROFLEX,
    ALVEOLO_PALATAL,
    LABIAL_PALATAL,
    PALATAL,
    PALATAL_VELAR,
    LABIAL_VELAR,
    VELAR,
    UVULAR,
    PHARYNGEAL,
    EPIGLOTTAL,
    GLOTTAL,
  };

  /**
   * Manner of articulation for consonants.
   */
  enum class MannerOfArticulation
  {
    NASAL,
    PLOSIVE,
    SIBILANT_FRICATIVE,
    NON_SIBILANT_FRICATIVE,
    APPROXIMANT,
    FLAP,
    TRILL,
    LATERAL_FRICATIVE,
    LATERAL_APPROXIMANT,
    LATERAL_FLAP,
    CLICK,
    IMPLOSIVE,
    EJECTIVE,
  };

  /**
   * Vowel height.
   */
  enum class VowelHeight
  {
    CLOSE,
    NEAR_CLOSE,
    CLOSE_MID,
    MID,
    OPEN_MID,
    NEAR_OPEN,
    OPEN,
  };

  /**
   * Horizontal vowel position.
   */
  enum class VowelBackness
  {
    FRONT,
    NEAR_FRONT,
    CENTRAL,
    NEAR_BACK,
    BACK,
  };

  /**
   * Vowel roundedness.
   */
  enum class VowelRoundedness
  {
    UNROUNDED,
    LESS_ROUNDED,
    ROUNDED,
    MORE_ROUNDED,
  };

  /**
   * A <em>phone</em> is a unit of speech sound.
   */
  class Phone
  {
  public:
    explicit Phone(PhoneType type);

    /**
     * @return The type of phone (consonant or vowel).
     */
    PhoneType type() const;

    /**
     * @return The phonation (voice intensity).
     */
    Phonation phonation() const;

    /**
     * Set the phonation.
     *
     */
    void phonation(Phonation phonation);

    /**
     * @return The place of articulation, for consonants.
     */
    PlaceOfArticulation place() const;

    /**
     * Set the place of articulation.
     *
     */
    void place(PlaceOfArticulation place);

    /**
     * @return The manner of articulation, for consonants.
     */
    MannerOfArticulation manner() const;

    /**
     * Set the manner of articulation.
     *
     */
    void manner(MannerOfArticulation manner);

    /**
     * @return The height, for vowels.
     */
    VowelHeight height() const;

    /**
     * Set the vowel height.
     *
     */
    void height(VowelHeight height);

    /**
     * @return The backness, for vowels.
     */
    VowelBackness backness() const;

    /**
     * Set the vowel backness.
     *
     */
    void backness(VowelBackness backness);

    /**
     * @return This vowel's roundedness.
     */
    VowelRoundedness roundedness() const;

    /**
     * Set this vowel's roundedness.
     *
     */
    void roundedness(VowelRoundedness roundedness);

    /**
     * Whether this vowel is rhotacized.
     */
    bool is_rhotic() const;

    /**
     * Set whether this vowel is rhotacized.
     *
     */
    void rhotic(bool rhotic);

    /**
     * @return Whether this phone is syllabic.
     */
    bool is_syllabic() const;

    /**
     * Set whether this phone is syllabic.
     *
     */
    void syllabic(bool syllabic);

  private:
    friend class Pronunciation;
    explicit Phone(std::uint16_t repr);

    std::uint16_t m_repr;
  };

  /**
   * A phonetic pronunciation.
   */
  class Pronunciation
  {
  public:
    /** An iterator over the phones in a pronunciation. */
    using iterator = std::vector<Phone>::const_iterator;
    /** Size type. */
    using size_type = std::vector<Phone>::size_type;

    virtual ~Pronunciation() = 0;

    Pronunciation(const Pronunciation& other) = default;
    Pronunciation(Pronunciation&& other) = default;
    Pronunciation& operator=(const Pronunciation& other) = default;
    Pronunciation& operator=(Pronunciation&& other) = default;

    /**
     * @return An iterator to the first @c Phone.
     */
    iterator begin() const;

    /**
     * @return An iterator past the last @c Phone.
     */
    iterator end() const;

    /**
     * @return Whether this pronunciation is empty.
     */
    bool empty() const;

    /**
     * @return The number of phones in this pronunciation.
     */
    size_type size() const;

    /**
     * @return The IPA form of this prununciation.
     */
    std::string to_ipa() const;

  protected:
    Pronunciation();
    Pronunciation(std::u16string ipa);

    std::u16string m_ipa;
    std::vector<Phone> m_phones;
  };

  /**
   * A phonetic pronunciation dependent on the English Language.
   */
  class EnPronunciation: public Pronunciation
  {
  public:
    virtual ~EnPronunciation();

    EnPronunciation(const EnPronunciation& other);
    EnPronunciation(EnPronunciation&& other);
    EnPronunciation& operator=(const EnPronunciation& other);
    EnPronunciation& operator=(EnPronunciation&& other);

    /**
     * Parse an Arpabet pronunciation.
     */
    static EnPronunciation from_arpabet(const std::vector<std::string>& arpabet);

    /**
     * Parse an IPA pronunciation.
     */
    static EnPronunciation from_ipa(const xtd::string_view ipa);

    /**
     * Carve out a subrange of this @c Pronunciation.
     *
     * @param first  An iterator to the start of the desired range.
     * @param last  An iterator past the end of the desired range.
     * @return A @c Pronunciation corresponding to the given range.
     */
    EnPronunciation subrange(iterator first, iterator last) const;

  private:
    EnPronunciation();
    EnPronunciation(std::u16string ipa);
  };

  /**
   * Convert a @c Pronunciation to a @c std::string.
   */
  std::string to_string(const Pronunciation& pron);

  /**
   * Print a @c Pronuncation.
   */
  std::ostream& operator<<(std::ostream& stream, const Pronunciation& pron);
}
}

#endif // MALUUBA_SPEECH_PRONUNCIATION_HPP
