/**
 * @file
 * Speech implementation details.
 *
 * @author Benedicte Pierrejean
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_PRONUNCIATION_IMPL_HPP
#define MALUUBA_SPEECH_PRONUNCIATION_IMPL_HPP

#include "maluuba/speech/pronunciation.hpp"
#include <cstdint>

namespace maluuba
{
namespace speech
{
namespace internal
{
  // Phones are implemented as "manual" bitfields.
  // Bits are layed out like this:
  //
  // struct Phone (14) {
  //   PhoneType type : 1;
  //   Phonation phonation : 3;
  //   bool syllabic : 1;
  //
  //   union {
  //     struct Consonant (8) {
  //       PlaceOfArticulation place : 4;
  //       MannerOfArticulation manner : 4;
  //     };
  //     struct Vowel (9) {
  //       VowelHeight height : 3;
  //       VowelBackness backness : 3;
  //       VowelRoundedness roundedness : 2;
  //       bool rhotic : 1;
  //     };
  //   };
  // }

  static constexpr std::uint16_t type_start = 0;
  static constexpr std::uint16_t type_end = type_start + 1;

  static constexpr std::uint16_t phonation_start = type_end;
  static constexpr std::uint16_t phonation_end = phonation_start + 3;

  static constexpr std::uint16_t syllabic_start = phonation_end;
  static constexpr std::uint16_t syllabic_end = syllabic_start + 1;

  static constexpr std::uint16_t place_start = syllabic_end;
  static constexpr std::uint16_t place_end = place_start + 4;

  static constexpr std::uint16_t manner_start = place_end;
  static constexpr std::uint16_t manner_end = manner_start + 4;

  static constexpr std::uint16_t height_start = syllabic_end;
  static constexpr std::uint16_t height_end = height_start + 3;

  static constexpr std::uint16_t backness_start = height_end;
  static constexpr std::uint16_t backness_end = backness_start + 3;

  static constexpr std::uint16_t roundedness_start = backness_end;
  static constexpr std::uint16_t roundedness_end = roundedness_start + 2;

  static constexpr std::uint16_t rhotic_start = roundedness_end;
  static constexpr std::uint16_t rhotic_end = rhotic_start + 1;

  /**
   * Compute a bitmask from a bit range.
   */
  constexpr std::uint16_t
  phone_mask(std::uint16_t start, std::uint16_t end)
  {
    return (1 << (end - start)) - 1;
  }

  /**
   * Decode some bits in the phone representation.
   */
  template <typename T>
  constexpr T
  phone_decode(std::uint16_t repr, std::uint16_t start, std::uint16_t end)
  {
    return static_cast<T>((repr >> start) & phone_mask(start, end));
  }

  /**
   * Encode some bits in the phone representation.
   */
  template <typename T>
  constexpr std::uint16_t
  phone_encode(T t, std::uint16_t start)
  {
    return static_cast<std::uint16_t>(t) << start;
  }

  /**
   * Encode some bits in the phone representation.
   */
  template <typename T>
  constexpr std::uint16_t
  phone_encode(std::uint16_t repr, T t, std::uint16_t start, std::uint16_t end)
  {
    return (repr & ~(phone_mask(start, end) << start))
      | phone_encode(t, start);
  }

  /**
   * Create a phone representation for a consonant.
   */
  constexpr std::uint16_t
  consonant(Phonation phonation, PlaceOfArticulation place, MannerOfArticulation manner)
  {
    return phone_encode(PhoneType::CONSONANT, type_start)
      | phone_encode(phonation, phonation_start)
      | phone_encode(place, place_start)
      | phone_encode(manner, manner_start);
  }

  /**
   * Create a phone representation for a vowel.
   */
  constexpr std::uint16_t
  vowel(VowelHeight height, VowelBackness backness, VowelRoundedness roundedness, bool rhotic = false)
  {
    return phone_encode(PhoneType::VOWEL, type_start)
      | phone_encode(Phonation::MODAL, phonation_start)
      | phone_encode(true, syllabic_start)
      | phone_encode(height, height_start)
      | phone_encode(backness, backness_start)
      | phone_encode(roundedness, roundedness_start)
      | phone_encode(rhotic, rhotic_start);
  }
}
}
}

#endif // MALUUBA_SPEECH_PRONUNCIATION_IMPL_HPP
