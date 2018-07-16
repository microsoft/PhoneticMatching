/**
 * @file
 * Utilities for working with Unicode text.
 *
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_UNICODE_HPP
#define MALUUBA_UNICODE_HPP

#include "maluuba/xtd/string_view.hpp"
#include <string>

namespace maluuba
{

  template <typename String>
  String unicode_cast(const xtd::string_view utf8);

  template <typename String>
  String unicode_cast(const xtd::u16string_view utf16);

  /**
   * No-op conversion for UTF-8.
   *
   * @param utf8  A UTF-8 encoded string.
   * @return The equivalent UTF-8 encoded string.
   */
  template <>
  std::string unicode_cast<std::string>(const xtd::string_view utf8);

  /**
   * Convert the given UTF-16 encoded string to UTF-8.
   *
   * @param utf16  A UTF-16 encoded string.
   * @return The equivalent UTF-8 encoded string.
   */
  template <>
  std::string unicode_cast<std::string>(const xtd::u16string_view utf16);

  /**
   * Convert the given UTF-8 encoded string to UTF-16.
   *
   * @param utf8  A UTF-8 encoded string.
   * @return The equivalent UTF-16 encoded string.
   */
  template <>
  std::u16string unicode_cast<std::u16string>(const xtd::string_view utf8);

  /**
   * No-op conversion for UTF-16.
   *
   * @param utf16  A UTF-16 encoded string.
   * @return The equivalent UTF-16 encoded string.
   */
  template <>
  std::u16string unicode_cast<std::u16string>(const xtd::u16string_view utf16);
}

#endif // MALUUBA_UNICODE_HPP
