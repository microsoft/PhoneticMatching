// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/pronunciation/impl.hpp"
#include "maluuba/speech/pronunciation.hpp"
#include "maluuba/debug.hpp"
#include <cstdint>

namespace maluuba
{
namespace speech
{
  using namespace internal;

  namespace
  {
    void
    check_consonant(const Phone& phone)
    {
      check_logic(phone.type() == PhoneType::CONSONANT, "This phone is not a consonant.");
    }

    void
    check_vowel(const Phone& phone)
    {
      check_logic(phone.type() == PhoneType::VOWEL, "This phone is not a vowel.");
    }
  }

  Phone::Phone(PhoneType type)
    : Phone{phone_encode(type, type_start)}
  { }

  Phone::Phone(std::uint16_t repr)
    : m_repr{repr}
  { }

  PhoneType
  Phone::type() const
  {
    return phone_decode<PhoneType>(m_repr, type_start, type_end);
  }

  Phonation
  Phone::phonation() const
  {
    return phone_decode<Phonation>(m_repr, phonation_start, phonation_end);
  }

  void
  Phone::phonation(Phonation phonation)
  {
    m_repr = phone_encode(m_repr, phonation, phonation_start, phonation_end);
  }

  PlaceOfArticulation
  Phone::place() const
  {
    check_consonant(*this);
    return phone_decode<PlaceOfArticulation>(m_repr, place_start, place_end);
  }

  void
  Phone::place(PlaceOfArticulation place)
  {
    check_consonant(*this);
    m_repr = phone_encode(m_repr, place, place_start, place_end);
  }

  MannerOfArticulation
  Phone::manner() const
  {
    check_consonant(*this);
    return phone_decode<MannerOfArticulation>(m_repr, manner_start, manner_end);
  }

  void
  Phone::manner(MannerOfArticulation manner)
  {
    check_consonant(*this);
    m_repr = phone_encode(m_repr, manner, manner_start, manner_end);
  }

  VowelHeight
  Phone::height() const
  {
    check_vowel(*this);
    return phone_decode<VowelHeight>(m_repr, height_start, height_end);
  }

  void
  Phone::height(VowelHeight height)
  {
    check_vowel(*this);
    m_repr = phone_encode(m_repr, height, height_start, height_end);
  }

  VowelBackness
  Phone::backness() const
  {
    check_vowel(*this);
    return phone_decode<VowelBackness>(m_repr, backness_start, backness_end);
  }

  void
  Phone::backness(VowelBackness backness)
  {
    check_vowel(*this);
    m_repr = phone_encode(m_repr, backness, backness_start, backness_end);
  }

  VowelRoundedness
  Phone::roundedness() const
  {
    check_vowel(*this);
    return phone_decode<VowelRoundedness>(m_repr, roundedness_start, roundedness_end);
  }

  void
  Phone::roundedness(VowelRoundedness roundedness)
  {
    check_vowel(*this);
    m_repr = phone_encode(m_repr, roundedness, roundedness_start, roundedness_end);
  }

  bool
  Phone::is_rhotic() const
  {
    check_vowel(*this);
    return phone_decode<bool>(m_repr, rhotic_start, rhotic_end);
  }

  void
  Phone::rhotic(bool rhotic)
  {
    check_vowel(*this);
    m_repr = phone_encode(m_repr, rhotic, rhotic_start, rhotic_end);
  }

  bool
  Phone::is_syllabic() const
  {
    return phone_decode<bool>(m_repr, syllabic_start, syllabic_end);
  }

  void
  Phone::syllabic(bool syllabic)
  {
    m_repr = phone_encode(m_repr, syllabic, syllabic_start, syllabic_end);
  }
}
}
