/**
 * @file
 * String Distance wrapped in NodeJS.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_NODEJS_STRINGDISTANCE_HPP
#define MALUUBA_SPEECH_NODEJS_STRINGDISTANCE_HPP

#include "maluuba/levenshtein.hpp"
#include <node.h>
#include <node_object_wrap.h>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  class StringDistance: public node::ObjectWrap
  {
  public:
    static void Init(v8::Local<v8::Object> exports);
    static v8::Local<v8::FunctionTemplate> type(v8::Isolate* isolate);

    const LevenshteinDistance<>& distance() const;

  private:
    explicit StringDistance(LevenshteinDistance<> distance);
    ~StringDistance();

    static void New(const v8::FunctionCallbackInfo<v8::Value>& args);
    static void Distance(const v8::FunctionCallbackInfo<v8::Value>& args);
    static v8::Persistent<v8::Function> s_constructor;
    static v8::Persistent<v8::FunctionTemplate> s_type;
    LevenshteinDistance<> m_distance;
  };
}
}
}

#endif // MALUUBA_SPEECH_NODEJS_STRINGDISTANCE_HPP
