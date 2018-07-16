// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/nodejs/enpronunciation.hpp"
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
    getIpa(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::EnPronunciation>(info.Holder());
      auto ipa = obj->pronunciation().to_ipa();
      info.GetReturnValue().Set(v8::String::NewFromUtf8(isolate, ipa.data(), v8::String::kNormalString, ipa.length()));
    }

    void
    getPhones(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::EnPronunciation>(info.Holder());
      const auto& phones = obj->pronunciation();
      auto array = v8::Array::New(isolate);

      auto context = isolate->GetCurrentContext();
      std::size_t i = 0;
      for (const auto& phone : phones) {
        auto obj = new Phone(phone);

        const auto argc = 1;
        v8::Local<v8::Value> argv[argc] = { v8::External::New(isolate, obj) };
        auto instance = Phone::constructor(isolate)->NewInstance(context, argc, argv).ToLocalChecked();
        array->Set(i++, instance);
      }
      info.GetReturnValue().Set(array);
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

  v8::Persistent<v8::Function> EnPronunciation::s_constructor;
  v8::Persistent<v8::FunctionTemplate> EnPronunciation::s_type;

  EnPronunciation::EnPronunciation(speech::EnPronunciation pronunciation)
    : m_pronunciation{std::move(pronunciation)}
  { }

  v8::Local<v8::Function>
  EnPronunciation::constructor(v8::Isolate* isolate)
  {
    return v8::Local<v8::Function>::New(isolate, s_constructor);
  }

  v8::Local<v8::FunctionTemplate>
  EnPronunciation::type(v8::Isolate* isolate)
  {
    return s_type.Get(isolate);
  }

  void
  EnPronunciation::Init(v8::Local<v8::Object> exports)
  {
    auto isolate = exports->GetIsolate();

    auto tpl = v8::FunctionTemplate::New(isolate, New);
    tpl->SetClassName(v8::String::NewFromUtf8(isolate, "EnPronunciation"));
    tpl->InstanceTemplate()->SetInternalFieldCount(1);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "ipa"), getIpa, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "phones"), getPhones, setThrow);

    s_constructor.Reset(isolate, tpl->GetFunction());
    s_type.Reset(isolate, tpl);

    auto otpl = v8::ObjectTemplate::New(isolate);
    otpl->Set(v8::String::NewFromUtf8(isolate, "fromIpa"), v8::FunctionTemplate::New(isolate, FromIpa));
    otpl->Set(v8::String::NewFromUtf8(isolate, "fromArpabet"), v8::FunctionTemplate::New(isolate, FromArpabet));

    exports->Set(v8::String::NewFromUtf8(isolate, "EnPronunciation"),
                otpl->NewInstance());
  }

  void
  EnPronunciation::New(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (!args[0]->IsExternal()) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected use as EnPronunciation.fromIpa() or similar.")));
      return;
    }

    auto self = args.Holder();
    auto external = args[0].As<v8::External>();
    auto obj = static_cast<EnPronunciation*>(external->Value());
    obj->Wrap(self);
    args.GetReturnValue().Set(self);
  }

  void
  EnPronunciation::FromIpa(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (args.Length() < 1) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected 1 argument.")));
      return;
    }

    if (!args[0]->IsString()) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected argument to be a string.")));
      return;
    }

    v8::String::Utf8Value ipa{isolate, args[0]};
    try {
      auto pronunciation = speech::EnPronunciation::from_ipa(*ipa);
      auto obj = new EnPronunciation(std::move(pronunciation));

      const auto argc = 1;
      v8::Local<v8::Value> argv[argc] = { v8::External::New(isolate, obj) };
      auto context = isolate->GetCurrentContext();
      auto instance = constructor(isolate)->NewInstance(context, argc, argv).ToLocalChecked();
      args.GetReturnValue().Set(instance);
    } catch (const std::exception& e) {
      isolate->ThrowException(v8::Exception::Error(
          v8::String::NewFromUtf8(isolate, e.what())));
      return;
    }
  }

  void
  EnPronunciation::FromArpabet(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (args.Length() < 1) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected 1 argument.")));
      return;
    }

    if (!args[0]->IsArray()) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected argument to be a string[].")));
      return;
    }

    auto array_arg = args[0].As<v8::Array>();
    std::vector<std::string> arpabet;
    for (uint32_t i = 0; i < array_arg->Length(); ++i) {
      auto wrap_phoneme = array_arg->Get(i);
      if (!wrap_phoneme->IsString()) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected argument to be a string[].")));
        return;
      }
      v8::String::Utf8Value phoneme{isolate, wrap_phoneme};
      arpabet.emplace_back(*phoneme);
    }

    try {
      auto pronunciation = speech::EnPronunciation::from_arpabet(arpabet);
      auto obj = new EnPronunciation(std::move(pronunciation));

      const auto argc = 1;
      v8::Local<v8::Value> argv[argc] = { v8::External::New(isolate, obj) };
      auto context = isolate->GetCurrentContext();
      auto instance = constructor(isolate)->NewInstance(context, argc, argv).ToLocalChecked();
      args.GetReturnValue().Set(instance);
    } catch (const std::exception& e) {
      isolate->ThrowException(v8::Exception::Error(
          v8::String::NewFromUtf8(isolate, e.what())));
      return;
    }
  }

  const speech::EnPronunciation&
  EnPronunciation::pronunciation() const
  {
    return m_pronunciation;
  }
}
}
}
