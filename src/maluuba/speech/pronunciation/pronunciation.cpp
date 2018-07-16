// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/pronunciation.hpp"
#include "maluuba/unicode.hpp"

namespace maluuba
{
namespace speech
{
  Pronunciation::Pronunciation() = default;

  Pronunciation::~Pronunciation() = default;

  Pronunciation::iterator
  Pronunciation::begin() const
  {
    return m_phones.begin();
  }

  Pronunciation::iterator
  Pronunciation::end() const
  {
    return m_phones.end();
  }

  bool
  Pronunciation::empty() const
  {
    return m_phones.empty();
  }

  Pronunciation::size_type
  Pronunciation::size() const
  {
    return m_phones.size();
  }

  std::string
  Pronunciation::to_ipa() const
  {
    return unicode_cast<std::string>(m_ipa);
  }

  EnPronunciation::~EnPronunciation() = default;

  EnPronunciation::EnPronunciation(const EnPronunciation& other) = default;

  EnPronunciation::EnPronunciation(EnPronunciation&& other) = default;

  EnPronunciation&
  EnPronunciation::operator=(const EnPronunciation& other) = default;

  EnPronunciation&
  EnPronunciation::operator=(EnPronunciation&& other) = default;

  EnPronunciation::EnPronunciation()
    : Pronunciation{}
  { }

  EnPronunciation::EnPronunciation(std::u16string ipa)
    : Pronunciation{ipa}
  { }

  std::string
  to_string(const Pronunciation& pron)
  {
    return pron.to_ipa();
  }

  std::ostream&
  operator<<(std::ostream& stream, const Pronunciation& pron)
  {
    return stream << to_string(pron);
  }
}
}
