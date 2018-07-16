// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#ifndef MALUUBA_XTD_OPTIONAL_HPP
#define MALUUBA_XTD_OPTIONAL_HPP
#if __cplusplus >= 201703L

#include <optional>
namespace maluuba
{
namespace xtd
{
  using namespace ::std;
}
}

#else
#include <experimental/optional>
namespace maluuba
{
namespace xtd
{
  using namespace ::std;
  using namespace ::std::experimental;
}
}

#endif // __cplusplus
#endif // MALUUBA_XTD_OPTIONAL_HPP
