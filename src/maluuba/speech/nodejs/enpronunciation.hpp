/**
 * @file
 * English Pronunciation wrapped in NodeJS.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_NODEJS_ENPRONUNCIATION_HPP
#define MALUUBA_SPEECH_NODEJS_ENPRONUNCIATION_HPP

#include "maluuba/speech/pronunciation.hpp"
#include <node.h>
#include <node_object_wrap.h>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  class EnPronunciation: public node::ObjectWrap
  {
  public:
    static void Init(v8::Local<v8::Object> exports);
    static v8::Local<v8::Function> constructor(v8::Isolate* isolate);
    static v8::Local<v8::FunctionTemplate> type(v8::Isolate* isolate);

    EnPronunciation(speech::EnPronunciation pronunciation);
    const speech::EnPronunciation& pronunciation() const;

  private:
    static void New(const v8::FunctionCallbackInfo<v8::Value>& args);
    static void FromIpa(const v8::FunctionCallbackInfo<v8::Value>& args);
    static void FromArpabet(const v8::FunctionCallbackInfo<v8::Value>& args);
    static v8::Persistent<v8::Function> s_constructor;
    static v8::Persistent<v8::FunctionTemplate> s_type;
    speech::EnPronunciation m_pronunciation;
  };
}
}
}

#endif // MALUUBA_SPEECH_NODEJS_ENPRONUNCIATION_HPP
