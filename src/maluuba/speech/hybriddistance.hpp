/**
 * @file
 * Hybrid distance combining strings and phonemes.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_HYBRID_DISTANCE_HPP
#define MALUUBA_SPEECH_HYBRID_DISTANCE_HPP

#include "maluuba/speech/phoneticdistance.hpp"
#include "maluuba/debug.hpp"
#include "maluuba/levenshtein.hpp"

namespace maluuba
{
namespace speech
{
  /**
   * Compute the  phonetic distance between English pronunciations.
   *
   * @tparam LevenshteinDistance<>
   * @tparam EnPhoneticDistance
   */
  template <typename StringDistance = LevenshteinDistance<>, typename PhoneticDistance = EnPhoneticDistance>
  class HybridDistance
  {
  public:
    /**
     * Construct a new Hybrid Distance metric.
     *
     * @param phonetic_weight_percentage Between 0 and 1. Weighting trade-off between the phonetic
     *  distance and the lexical distance scores. 1 meaning 100% phonetic score and 0% lexical score.
     */
    HybridDistance(double phonetic_weight_percentage)
      : m_phonetic_weight_percentage{phonetic_weight_percentage}
    {
      check(m_phonetic_weight_percentage >= 0.0 && m_phonetic_weight_percentage <= 1.0,
        "require 0 <= phonetic_weight_percentage <= 1");
    }

    /**
     * @return The phonetic weight percentage being used.
     */
    double
    phonetic_weight_percentage() const
    {
      return m_phonetic_weight_percentage;
    }

    /**
     * @return The combined phonetic and lexical distance between @p a and @p b.
     */
    template <typename StringInput, typename PhoneticInput>
    double operator()(const StringInput& a_string, const PhoneticInput& a_pronunciation, const StringInput& b_string, const PhoneticInput& b_pronunciation) const
    {
      double string_weight = 0.0;
      double phonetic_weight = 0.0;
      if (m_phonetic_weight_percentage > 0.0) {
        phonetic_weight = m_phonetic_weight_percentage * m_phonetic_distance(a_pronunciation, b_pronunciation);
      }
      if (m_phonetic_weight_percentage < 1.0) {
        string_weight = (1.0 - m_phonetic_weight_percentage) * m_string_distance(a_string, b_string);
      }
      return phonetic_weight + string_weight;
    }

  private:
    double m_phonetic_weight_percentage;
    StringDistance m_string_distance;
    PhoneticDistance m_phonetic_distance;
  };
}
}

#endif // MALUUBA_SPEECH_HYBRID_DISTANCE_HPP
