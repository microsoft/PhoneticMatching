/**
 * @file
 * Phonetic distance.
 *
 * @author Benedicte Pierrejean
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_PHONETIC_DISTANCE_HPP
#define MALUUBA_SPEECH_PHONETIC_DISTANCE_HPP

#include "maluuba/speech/pronunciation.hpp"
#include <vector>

namespace maluuba
{
namespace speech
{
  /**
   * A phoneme embedded in a metric space for similarity measurement.
   */
  class PhonemeVector
  {
  public:
    /**
     * Initialize a @c PhonemeVector.
     *
     * @param v  The three-dimensional embedding of this phoneme.
     * @param syllabic  Whether the phoneme is syllabic.
     */
    PhonemeVector(float v[3], bool syllabic);

    /**
     * @return The @p i'th dimension (out of 3) of the vector representation.
     */
    float operator[](std::size_t i) const;

    /**
     * @return Whether this phoneme is syllabic.
     */
    bool is_syllabic() const;

  private:
    friend bool operator==(const PhonemeVector&, const PhonemeVector&);

    float m_v[3];
    bool m_syllabic;
  };

  bool operator==(const PhonemeVector& lhs, const PhonemeVector& rhs);

  /**
   * An entire pronunciation embedded in a metric space.
   */
  using PronunciationVector = std::vector<PhonemeVector>;

  /**
   * Compute the vector representation of a pronunciation for similarity
   * measurement.
   *
   * @param pronunciation  The pronunciation to embed.
   * @return A metric space embedding of the pronunciation.
   */
  PronunciationVector phonetic_embedding(const Pronunciation& pronunciation);

  /**
   * Compute the phonetic distance between pronunciations.
   */
  class PhoneticDistance
  {
  public:
    virtual ~PhoneticDistance() = 0;

  protected:
    /**
     * @return The phonetic distance of phonemes between @p a and @p b.
     */
    double operator()(const PronunciationVector& a, const PronunciationVector& b) const;
  };

  /**
   * Compute the phonetic distance between English pronunciations.
   */
  class EnPhoneticDistance: public PhoneticDistance
  {
  public:
    virtual ~EnPhoneticDistance();

    /**
     * @return The phonetic distance between English pronuncations @p a and @p b.
     */
    double operator()(const EnPronunciation& a, const EnPronunciation& b) const;
  };
}
}

#endif // MALUUBA_SPEECH_PHONETIC_DISTANCE_HPP
