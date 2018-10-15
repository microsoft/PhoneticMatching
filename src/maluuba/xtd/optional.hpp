// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#ifndef MALUUBA_XTD_OPTIONAL_HPP
#define MALUUBA_XTD_OPTIONAL_HPP

#if __cplusplus >= 201703L && __has_include(<optional>)

#include <optional>

namespace maluuba
{
namespace xtd
{
  using std::optional;
  using std::bad_optional_access;
  using std::nullopt_t;
  using std::nullopt;
  using std::make_optional;
}
}

#else

#include <experimental/optional>

namespace maluuba
{
namespace xtd
{
  using std::experimental::optional;
  using std::experimental::bad_optional_access;
  using std::experimental::nullopt_t;
  using std::experimental::nullopt;
  using std::experimental::make_optional;
}
}

#endif // __cplusplus

#endif // MALUUBA_XTD_OPTIONAL_HPP
