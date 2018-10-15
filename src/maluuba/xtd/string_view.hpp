// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#ifndef MALUUBA_XTD_STRING_VIEW_HPP
#define MALUUBA_XTD_STRING_VIEW_HPP

#if __cplusplus >= 201703L

#include <string_view>

namespace maluuba
{
namespace xtd
{
  using std::basic_string_view;
  using std::string_view;
  using std::wstring_view;
  using std::u16string_view;
  using std::u32string_view;
}
}

#else

#include <experimental/string_view>

namespace maluuba
{
namespace xtd
{
  using std::experimental::basic_string_view;
  using std::experimental::string_view;
  using std::experimental::wstring_view;
  using std::experimental::u16string_view;
  using std::experimental::u32string_view;
}
}

#endif // __cplusplus

#endif // MALUUBA_XTD_STRING_VIEW_HPP
