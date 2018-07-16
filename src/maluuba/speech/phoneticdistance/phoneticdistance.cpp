// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/phoneticdistance.hpp"

namespace maluuba
{
namespace speech
{
  PhoneticDistance::~PhoneticDistance() = default;

  EnPhoneticDistance::~EnPhoneticDistance() = default;

  double
  EnPhoneticDistance::operator()(const EnPronunciation& a, const EnPronunciation& b) const
  {
    return PhoneticDistance::operator()(phonetic_embedding(a), phonetic_embedding(b));
  }
}
}
