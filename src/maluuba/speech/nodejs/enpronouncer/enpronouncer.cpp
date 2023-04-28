// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/nodejs/enpronouncer.hpp"
#include "maluuba/speech/nodejs/enpronunciation.hpp"
#include <utility>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  v8::Persistent<v8::Function> EnPronouncer::s_constructor;

  EnPronouncer::EnPronouncer(speech::EnPronouncer pronouncer)
    : m_pronouncer{std::move(pronouncer)}
  { }

  EnPronouncer::~EnPronouncer() = default;

  void
  EnPronouncer::Init(v8::Local<v8::Object> exports)
  {
    auto isolate = exports->GetIsolate();
    v8::Local<v8::Context> context = isolate->GetCurrentContext();

    auto tpl = v8::FunctionTemplate::New(isolate, New);
    tpl->SetClassName(v8::String::NewFromUtf8(isolate, "EnPronouncer", v8::NewStringType::kNormal).ToLocalChecked());
    tpl->InstanceTemplate()->SetInternalFieldCount(1);

    NODE_SET_PROTOTYPE_METHOD(tpl, "pronounce", Pronounce);

    s_constructor.Reset(isolate, tpl->GetFunction(context).ToLocalChecked());
    exports->Set(context, v8::String::NewFromUtf8(isolate, "EnPronouncer", v8::NewStringType::kNormal).ToLocalChecked(), tpl->GetFunction(context).ToLocalChecked());
  }

  void
  EnPronouncer::New(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (args.IsConstructCall()) {
      speech::EnPronouncer pronouncer{};
      auto obj = new EnPronouncer(std::move(pronouncer));
      obj->Wrap(args.This());
      args.GetReturnValue().Set(args.This());
    } else {
      isolate->ThrowException(v8::Exception::SyntaxError(
        v8::String::NewFromUtf8(isolate, "Not invoked as constructor, change to: `new EnPronouncer()`", v8::NewStringType::kNormal).ToLocalChecked()));
      return;
    }
  }

  void
  EnPronouncer::Pronounce(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (args.Length() < 1) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected 1 argument.", v8::NewStringType::kNormal).ToLocalChecked()));
      return;
    }

    if (!args[0]->IsString()) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected argument to be a string.", v8::NewStringType::kNormal).ToLocalChecked()));
      return;
    }

    auto obj = ObjectWrap::Unwrap<EnPronouncer>(args.Holder());
    v8::String::Utf8Value phrase{isolate, args[0]};
    try {
      auto pronunciation = obj->pronouncer().pronounce(*phrase);

      auto wrap = new EnPronunciation(std::move(pronunciation));
      const auto argc = 1;
      v8::Local<v8::Value> argv[argc] = { v8::External::New(isolate, wrap) };
      auto context = isolate->GetCurrentContext();
      auto instance = EnPronunciation::constructor(isolate)->NewInstance(context, argc, argv).ToLocalChecked();
      args.GetReturnValue().Set(instance);
    } catch (const std::exception& e) {
      isolate->ThrowException(v8::Exception::Error(
          v8::String::NewFromUtf8(isolate, e.what(), v8::NewStringType::kNormal).ToLocalChecked()));
      return;
    }
  }

  const speech::EnPronouncer&
  EnPronouncer::pronouncer() const
  {
    return m_pronouncer;
  }
}
}
}
