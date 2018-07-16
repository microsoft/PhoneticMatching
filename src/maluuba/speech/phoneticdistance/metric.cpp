// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/phoneticdistance.hpp"
#include "maluuba/levenshtein.hpp"
#include <cmath>

// The vector representation of English phonemes used here is described in
// Li & MacWhinneey (2002). PatPho: A phonological pattern generator for neural networks
// http://blclab.org/wp-content/uploads/2013/02/patpho.pdf

namespace maluuba
{
namespace speech
{
  namespace
  {
    /** Convert a consonant to a @c PhonemeVector. */
    PhonemeVector
    consonant_to_vector(const Phone& phone)
    {
      float v[3]{};

      switch (phone.phonation()) {
        case Phonation::VOICELESS:
        case Phonation::GLOTTAL_CLOSURE:
          v[0] = 1.000f;
          break;

        default:
          v[0] = 0.750f;
          break;
      }

      switch (phone.place()) {
        case PlaceOfArticulation::BILABIAL:
          v[1] = 0.450f;
          break;

        case PlaceOfArticulation::LABIODENTAL:
          v[1] = 0.528f;
          break;

        case PlaceOfArticulation::DENTAL:
          v[1] = 0.606f;
          break;

        case PlaceOfArticulation::ALVEOLAR:
          v[1] = 0.684f;
          break;

        case PlaceOfArticulation::PALATO_ALVEOLAR:
        case PlaceOfArticulation::RETROFLEX:
        case PlaceOfArticulation::ALVEOLO_PALATAL:
          v[1] = 0.762f;
          break;

        case PlaceOfArticulation::PALATAL:
        case PlaceOfArticulation::LABIAL_PALATAL:
        case PlaceOfArticulation::PALATAL_VELAR:
          v[1] = 0.841f;
          break;

        case PlaceOfArticulation::VELAR:
        case PlaceOfArticulation::LABIAL_VELAR:
        case PlaceOfArticulation::UVULAR:
          v[1] = 0.921f;
          break;

        case PlaceOfArticulation::PHARYNGEAL:
        case PlaceOfArticulation::EPIGLOTTAL:
        case PlaceOfArticulation::GLOTTAL:
          v[1] = 1.000f;
          break;
      }

      switch (phone.manner()) {
        case MannerOfArticulation::NASAL:
          v[2] = 0.644f;
          break;

        case MannerOfArticulation::PLOSIVE:
        case MannerOfArticulation::CLICK:
        case MannerOfArticulation::IMPLOSIVE:
        case MannerOfArticulation::EJECTIVE:
          v[2] = 0.733f;
          break;

        case MannerOfArticulation::SIBILANT_FRICATIVE:
        case MannerOfArticulation::NON_SIBILANT_FRICATIVE:
          v[2] = 0.822f;
          break;

        case MannerOfArticulation::APPROXIMANT:
        case MannerOfArticulation::FLAP:
        case MannerOfArticulation::TRILL:
          v[2] = 0.911f;
          break;

        case MannerOfArticulation::LATERAL_FRICATIVE:
        case MannerOfArticulation::LATERAL_APPROXIMANT:
        case MannerOfArticulation::LATERAL_FLAP:
          v[2] = 1.000f;
          break;
      }

      return {v, phone.is_syllabic()};
    }

    /** Convert a consonant to a @c PhonemeVector. */
    PhonemeVector
    vowel_to_vector(const Phone& phone)
    {
      float v[3]{};

      v[0] = 0.100f;

      switch (phone.backness()) {
        case VowelBackness::FRONT:
        case VowelBackness::NEAR_FRONT:
          v[1] = 0.100f;
          break;

        case VowelBackness::CENTRAL:
          v[1] = 0.175f;
          break;

        case VowelBackness::NEAR_BACK:
        case VowelBackness::BACK:
          v[1] = 0.250f;
          break;
      }

      switch (phone.height()) {
        case VowelHeight::CLOSE:
        case VowelHeight::NEAR_CLOSE:
          v[2] = 0.100f;
          break;

        case VowelHeight::CLOSE_MID:
          v[2] = 0.185f;
          break;

        case VowelHeight::MID:
          v[2] = 0.270f;
          break;

        case VowelHeight::OPEN_MID:
          v[2] = 0.355f;
          break;

        case VowelHeight::NEAR_OPEN:
        case VowelHeight::OPEN:
          v[2] = 0.444f;
          break;
      }

      return {v, phone.is_syllabic()};
    }

    /** Convert a @c Phone to a @c PhonemeVector. */
    PhonemeVector
    to_vector(const Phone& phone)
    {
      if (phone.type() == PhoneType::CONSONANT) {
        return consonant_to_vector(phone);
      } else {
        return vowel_to_vector(phone);
      }
    }

    /**
     * Substitution costs for phonemes.
     */
    struct PhonemeDistance
    {
      double
      operator()(const PhonemeVector& a, const PhonemeVector& b) const
      {
        // TODO: Precompute these or use a faster metric than L2
        auto sum_sq = 0.0;
        for (int i = 0; i < 3; ++i) {
          float diff = a[i] - b[i];
          sum_sq += diff * diff;
        }
        return std::sqrt(sum_sq);
      }
    };

    /**
     * Insertion/deletion cost for phonemes.
     */
    struct PhonemeCost
    {
      double
      operator()(const PhonemeVector& phoneme) const
      {
        if (phoneme.is_syllabic()) {
          return 0.5;
        } else {
          return 0.25;
        }
      }
    };
  }

  PhonemeVector::PhonemeVector(float v[3], bool syllabic)
    : m_v{v[0], v[1], v[2]}, m_syllabic{syllabic}
  { }

  float
  PhonemeVector::operator[](std::size_t i) const
  {
    return m_v[i];
  }

  bool
  PhonemeVector::is_syllabic() const
  {
    return m_syllabic;
  }

  bool
  operator==(const PhonemeVector& lhs, const PhonemeVector& rhs)
  {
    return lhs.m_v == rhs.m_v
        && lhs.m_syllabic == rhs.m_syllabic;
  }

  PronunciationVector
  phonetic_embedding(const Pronunciation& pronunciation)
  {
    PronunciationVector v;
    for (const auto& phone : pronunciation) {
      v.push_back(to_vector(phone));
    }
    return v;
  }

  double
  PhoneticDistance::operator()(const PronunciationVector& a, const PronunciationVector& b) const
  {
    LevenshteinDistance<PhonemeDistance, PhonemeCost> metric{PhonemeDistance{}, PhonemeCost{}};
    return metric(a, b);
  }
}
}
