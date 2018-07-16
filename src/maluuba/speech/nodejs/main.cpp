// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/nodejs/enhybriddistance.hpp"
#include "maluuba/speech/nodejs/enphoneticdistance.hpp"
#include "maluuba/speech/nodejs/enpronouncer.hpp"
#include "maluuba/speech/nodejs/enpronunciation.hpp"
#include "maluuba/speech/nodejs/fuzzymatcher.hpp"
#include "maluuba/speech/nodejs/match.hpp"
// #include "maluuba/speech/nodejs/performance.hpp"
#include "maluuba/speech/nodejs/phone.hpp"
#include "maluuba/speech/nodejs/stringdistance.hpp"
#include <node.h>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  namespace
  {
    void
    Init(v8::Local<v8::Object> exports, v8::Local<v8::Object> module)
    {
      // Performance::Init(module);
      EnHybridDistance::Init(exports);
      EnPhoneticDistance::Init(exports);
      FuzzyMatcher<speech::LinearFuzzyMatcher>::Init(exports, "FuzzyMatcher");
      FuzzyMatcher<speech::AcceleratedFuzzyMatcher>::Init(exports, "AcceleratedFuzzyMatcher");
      EnPronouncer::Init(exports);
      EnPronunciation::Init(exports);
      Match::Init(exports);
      Phone::Init(exports);
      StringDistance::Init(exports);
    }
  }

  NODE_MODULE(NODE_GYP_MODULE_NAME, Init)
}
}
}
