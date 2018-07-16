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
  using namespace ::std;
}
}

#else
#include <experimental/string_view>
namespace maluuba
{
namespace xtd
{
  using namespace ::std;
  using namespace ::std::experimental;
}
}

#endif // __cplusplus
#endif // MALUUBA_XTD_STRING_VIEW_HPP
