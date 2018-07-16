/**
 * @file
 * English Pronouncer wrapped in NodeJS.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_NODEJS_ENPRONOUNCER_HPP
#define MALUUBA_SPEECH_NODEJS_ENPRONOUNCER_HPP

#include "maluuba/speech/pronouncer.hpp"
#include <node.h>
#include <node_object_wrap.h>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  class EnPronouncer: public node::ObjectWrap
  {
  public:
    static void Init(v8::Local<v8::Object> exports);

    const speech::EnPronouncer& pronouncer() const;

  private:
    explicit EnPronouncer(speech::EnPronouncer pronouncer);
    ~EnPronouncer();

    static void New(const v8::FunctionCallbackInfo<v8::Value>& args);
    static void Pronounce(const v8::FunctionCallbackInfo<v8::Value>& args);
    static v8::Persistent<v8::Function> s_constructor;
    speech::EnPronouncer m_pronouncer;
  };
}
}
}

#endif // MALUUBA_SPEECH_NODEJS_ENPRONOUNCER_HPP
