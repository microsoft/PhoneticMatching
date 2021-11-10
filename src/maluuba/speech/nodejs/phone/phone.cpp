// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/nodejs/phone.hpp"
#include <utility>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  namespace
  {
    void
    getType(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Phone>(info.Holder());
      auto type = static_cast<uint32_t>(obj->phone().type());
      info.GetReturnValue().Set(v8::Integer::New(isolate, type));
    }

    void
    getPhonation(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Phone>(info.Holder());
      auto phonation = static_cast<uint32_t>(obj->phone().phonation());
      info.GetReturnValue().Set(v8::Integer::New(isolate, phonation));
    }

    void
    getPlace(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Phone>(info.Holder());
      // consonant
      try {
        auto place = static_cast<uint32_t>(obj->phone().place());
        info.GetReturnValue().Set(v8::Integer::New(isolate, place));
      } catch (const std::exception& e) {
        info.GetReturnValue().Set(v8::Undefined(isolate));
      }
    }

    void
    getManner(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Phone>(info.Holder());
      // consonant
      try {
        auto manner = static_cast<uint32_t>(obj->phone().manner());
        info.GetReturnValue().Set(v8::Integer::New(isolate, manner));
      } catch (const std::exception& e) {
        info.GetReturnValue().Set(v8::Undefined(isolate));
      }
    }

    void
    getHeight(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Phone>(info.Holder());
      // vowel
      try {
        auto height = static_cast<uint32_t>(obj->phone().height());
        info.GetReturnValue().Set(v8::Integer::New(isolate, height));
      } catch (const std::exception& e) {
        info.GetReturnValue().Set(v8::Undefined(isolate));
      }
    }

    void
    getBackness(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Phone>(info.Holder());
      // vowel
      try {
        auto backness = static_cast<uint32_t>(obj->phone().backness());
        info.GetReturnValue().Set(v8::Integer::New(isolate, backness));
      } catch (const std::exception& e) {
        info.GetReturnValue().Set(v8::Undefined(isolate));
      }
    }

    void
    getRoundedness(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Phone>(info.Holder());
      // vowel
      try {
        auto roundedness = static_cast<uint32_t>(obj->phone().roundedness());
        info.GetReturnValue().Set(v8::Integer::New(isolate, roundedness));
      } catch (const std::exception& e) {
        info.GetReturnValue().Set(v8::Undefined(isolate));
      }
    }

    void
    getRhotic(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Phone>(info.Holder());
      // vowel
      try {
        auto rhotic = obj->phone().is_rhotic();
        info.GetReturnValue().Set(v8::Boolean::New(isolate, rhotic));
      } catch (const std::exception& e) {
        info.GetReturnValue().Set(v8::Undefined(isolate));
      }
    }

    void
    getSyllabic(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Phone>(info.Holder());
      auto syllabic = obj->phone().is_syllabic();
      info.GetReturnValue().Set(v8::Boolean::New(isolate, syllabic));
    }

    void
    setThrow(v8::Local<v8::String> property, v8::Local<v8::Value> value, const v8::PropertyCallbackInfo<void>& info)
    {
      auto isolate = info.GetIsolate();
      isolate->ThrowException(v8::Exception::Error(
          v8::String::NewFromUtf8(isolate, "Object is immutable, setters not allowed.")));
      return;
    }
  }

  v8::Persistent<v8::Function> Phone::s_constructor;

  Phone::Phone(speech::Phone phone)
    : m_phone{std::move(phone)}
  { }

  v8::Local<v8::Function>
  Phone::constructor(v8::Isolate* isolate)
  {
    return v8::Local<v8::Function>::New(isolate, s_constructor);
  }

  void
  Phone::Init(v8::Local<v8::Object> exports)
  {
    auto isolate = exports->GetIsolate();
    v8::Local<v8::Context> context = isolate->GetCurrentContext();

    auto tpl = v8::FunctionTemplate::New(isolate, New);
    tpl->SetClassName(v8::String::NewFromUtf8(isolate, "Phone"));
    tpl->InstanceTemplate()->SetInternalFieldCount(1);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "type"), getType, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "phonation"), getPhonation, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "place"), getPlace, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "manner"), getManner, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "height"), getHeight, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "backness"), getBackness, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "roundedness"), getRoundedness, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "isRhotic"), getRhotic, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "isSyllabic"), getSyllabic, setThrow);

    s_constructor.Reset(isolate, tpl->GetFunction(context).ToLocalChecked());
  }

  void
  Phone::New(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (!args[0]->IsExternal()) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Not Expected to initialize directly, use EnPronunciation.")));
      return;
    }

    auto self = args.Holder();
    auto external = args[0].As<v8::External>();
    auto obj = static_cast<Phone*>(external->Value());
    obj->Wrap(self);
    args.GetReturnValue().Set(self);
  }

  const speech::Phone&
  Phone::phone() const
  {
    return m_phone;
  }
}
}
}
