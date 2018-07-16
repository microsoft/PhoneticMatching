/**
 * @file
 * Match wrapped in NodeJS.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_NODEJS_MATCH_HPP
#define MALUUBA_SPEECH_NODEJS_MATCH_HPP

#include "maluuba/speech/fuzzymatcher.hpp"
#include <node.h>
#include <node_object_wrap.h>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  class Match: public node::ObjectWrap
  {
    using NodeJsTarget = v8::UniquePersistent<v8::Value>;
    using MatchType = speech::FuzzyMatcher<NodeJsTarget>::Match;

  public:
    static void Init(v8::Local<v8::Object> exports);
    static v8::Local<v8::Function> constructor(v8::Isolate* isolate);

    Match(MatchType match);
    const MatchType& match() const;

  private:
    static void New(const v8::FunctionCallbackInfo<v8::Value>& args);
    static v8::Persistent<v8::Function> s_constructor;
    MatchType m_match;
  };
}
}
}

#endif // MALUUBA_SPEECH_NODEJS_MATCH_HPP
