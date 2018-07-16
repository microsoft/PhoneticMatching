// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/unicode.hpp"
#include <codecvt>
#include <locale>

namespace maluuba
{
  template <>
  std::string
  unicode_cast<std::string>(const xtd::string_view utf8)
  {
    return std::string{utf8};
  }

// https://stackoverflow.com/a/35103224
#if CODECVT_BUG
  template <>
  std::string
  unicode_cast<std::string>(const xtd::u16string_view utf16)
  {
    std::wstring_convert<std::codecvt_utf8_utf16<int16_t>, int16_t> convertor;
    auto p = reinterpret_cast<const int16_t*>(utf16.data());
    return convertor.to_bytes(p, p + utf16.size());
  }

  template <>
  std::u16string
  unicode_cast<std::u16string>(const xtd::string_view utf8)
  {
    std::wstring_convert<std::codecvt_utf8_utf16<int16_t>, int16_t> convertor;
    auto w = convertor.from_bytes(utf8.data(), utf8.data() + utf8.size());
    return {reinterpret_cast<const char16_t*>(w.data()), w.size()};
  }
#else
  template <>
  std::string
  unicode_cast<std::string>(const xtd::u16string_view utf16)
  {
    std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> convertor;
    return convertor.to_bytes(utf16.data(), utf16.data() + utf16.size());
  }

  template <>
  std::u16string
  unicode_cast<std::u16string>(const xtd::string_view utf8)
  {
    std::wstring_convert<std::codecvt_utf8_utf16<char16_t>, char16_t> convertor;
    return convertor.from_bytes(utf8.data(), utf8.data() + utf8.size());
  }
#endif // CODECVT_BUG

  template <>
  std::u16string
  unicode_cast<std::u16string>(const xtd::u16string_view utf16)
  {
    return std::u16string{utf16};
  }
}
