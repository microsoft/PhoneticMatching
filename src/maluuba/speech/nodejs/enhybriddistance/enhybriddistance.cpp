// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/nodejs/enhybriddistance.hpp"
#include "maluuba/speech/nodejs/enpronunciation.hpp"
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
    getphoneticWeightPercentage(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::EnHybridDistance>(info.Holder());
      auto phonetic_weight_percentage = obj->distance().phonetic_weight_percentage();
      info.GetReturnValue().Set(v8::Number::New(isolate, phonetic_weight_percentage));
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
  v8::Persistent<v8::Function> EnHybridDistance::s_constructor;
  v8::Persistent<v8::FunctionTemplate> EnHybridDistance::s_type;

  EnHybridDistance::EnHybridDistance(speech::HybridDistance<> distance)
    : m_distance{std::move(distance)}
  { }

  EnHybridDistance::~EnHybridDistance() = default;

  v8::Local<v8::FunctionTemplate>
  EnHybridDistance::type(v8::Isolate* isolate)
  {
    return s_type.Get(isolate);
  }

  void
  EnHybridDistance::Init(v8::Local<v8::Object> exports)
  {
    auto isolate = exports->GetIsolate();
    v8::Local<v8::Context> context = isolate->GetCurrentContext();

    auto tpl = v8::FunctionTemplate::New(isolate, New);
    tpl->SetClassName(v8::String::NewFromUtf8(isolate, "EnHybridDistance"));
    tpl->InstanceTemplate()->SetInternalFieldCount(1);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "phoneticWeightPercentage"), getphoneticWeightPercentage, setThrow);

    NODE_SET_PROTOTYPE_METHOD(tpl, "distance", Distance);

    s_constructor.Reset(isolate, tpl->GetFunction(context).ToLocalChecked());
    s_type.Reset(isolate, tpl);
    exports->Set(context, v8::String::NewFromUtf8(isolate, "EnHybridDistance"), tpl->GetFunction(context).ToLocalChecked());
  }

  void
  EnHybridDistance::New(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();
    v8::Local<v8::Context> context = isolate->GetCurrentContext();

    if (args.IsConstructCall()) {
      if (args.Length() < 1) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected 1 argument.")));
        return;
      }

      try {
        auto phonetic_weight_percentage = args[0]->NumberValue(context).ToChecked();
        speech::HybridDistance<> distance{phonetic_weight_percentage};
        auto obj = new EnHybridDistance(std::move(distance));
        obj->Wrap(args.This());
        args.GetReturnValue().Set(args.This());
      } catch (const std::exception& e) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Invalid phoneticWeightPercentage argument")));
        return;
      }
    } else {
      isolate->ThrowException(v8::Exception::SyntaxError(
        v8::String::NewFromUtf8(isolate, "Not invoked as constructor, change to: `new EnHybridDistance()`")));
      return;
    }
  }

  void
  EnHybridDistance::Distance(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (args.Length() < 2) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected 2 arguments.")));
      return;
    }

    if (!args[0]->IsObject() || !args[1]->IsObject()) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected arguments to be objects.")));
      return;
    }

    auto obj = ObjectWrap::Unwrap<EnHybridDistance>(args.Holder());
    auto phrase_key = v8::String::NewFromUtf8(isolate, "phrase");
    auto pronunciation_key = v8::String::NewFromUtf8(isolate, "pronunciation");
    auto en_pronunciation_type = EnPronunciation::type(isolate);
    try {
      auto a = args[0].As<v8::Object>();
      auto b = args[1].As<v8::Object>();
      auto a_wrap_string = a->Get(phrase_key);
      auto b_wrap_string = b->Get(phrase_key);
      if (!a_wrap_string->IsString() || !b_wrap_string->IsString()) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected 'phrase' to be strings.")));
        return;
      }
      std::string a_phrase{*v8::String::Utf8Value{isolate, a_wrap_string}};
      std::string b_phrase{*v8::String::Utf8Value{isolate, b_wrap_string}};
      auto a_wrap_pronunciation = a->Get(pronunciation_key);
      auto b_wrap_pronunciation = b->Get(pronunciation_key);
      if (!en_pronunciation_type->HasInstance(a_wrap_pronunciation) || !en_pronunciation_type->HasInstance(b_wrap_pronunciation)) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected 'pronunciation' to be EnPronunciation.")));
        return;
      }
      auto a_pronunciation = node::ObjectWrap::Unwrap<EnPronunciation>(a_wrap_pronunciation.As<v8::Object>())->pronunciation();
      auto b_pronunciation = node::ObjectWrap::Unwrap<EnPronunciation>(b_wrap_pronunciation.As<v8::Object>())->pronunciation();
      auto distance = obj->distance()(a_phrase, a_pronunciation, b_phrase, b_pronunciation);

      args.GetReturnValue().Set(v8::Number::New(isolate, distance));
    } catch (const std::exception& e) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected arguments to contain valid 'phrase' and 'pronunciation' entries.")));
      return;
    }
  }

  const speech::HybridDistance<>&
  EnHybridDistance::distance() const
  {
    return m_distance;
  }
}
}
}
