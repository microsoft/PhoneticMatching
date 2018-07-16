/**
 * @file
 * Phones wrapped in NodeJS.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_NODEJS_PHONE_HPP
#define MALUUBA_SPEECH_NODEJS_PHONE_HPP

#include "maluuba/speech/pronunciation.hpp"
#include <node.h>
#include <node_object_wrap.h>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  class Phone: public node::ObjectWrap
  {
  public:
    static void Init(v8::Local<v8::Object> exports);
    static v8::Local<v8::Function> constructor(v8::Isolate* isolate);

    Phone(speech::Phone phone);
    const speech::Phone& phone() const;

  private:
    static void New(const v8::FunctionCallbackInfo<v8::Value>& args);
    static v8::Persistent<v8::Function> s_constructor;
    speech::Phone m_phone;
  };
}
}
}

#endif // MALUUBA_SPEECH_NODEJS_PHONE_HPP
