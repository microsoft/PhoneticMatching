// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/nodejs/enphoneticdistance.hpp"
#include "maluuba/speech/nodejs/enpronunciation.hpp"
#include <utility>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  v8::Persistent<v8::Function> EnPhoneticDistance::s_constructor;
  v8::Persistent<v8::FunctionTemplate> EnPhoneticDistance::s_type;

  EnPhoneticDistance::EnPhoneticDistance(speech::EnPhoneticDistance distance)
    : m_distance{std::move(distance)}
  { }

  EnPhoneticDistance::~EnPhoneticDistance() = default;

  v8::Local<v8::FunctionTemplate>
  EnPhoneticDistance::type(v8::Isolate* isolate)
  {
    return s_type.Get(isolate);
  }

  void
  EnPhoneticDistance::Init(v8::Local<v8::Object> exports)
  {
    auto isolate = exports->GetIsolate();
    v8::Local<v8::Context> context = isolate->GetCurrentContext();

    auto tpl = v8::FunctionTemplate::New(isolate, New);
    tpl->SetClassName(v8::String::NewFromUtf8(isolate, "EnPhoneticDistance").ToLocalChecked());
    tpl->InstanceTemplate()->SetInternalFieldCount(1);

    NODE_SET_PROTOTYPE_METHOD(tpl, "distance", Distance);

    s_constructor.Reset(isolate, tpl->GetFunction(context).ToLocalChecked());
    s_type.Reset(isolate, tpl);
    exports->Set(context, v8::String::NewFromUtf8(isolate, "EnPhoneticDistance").ToLocalChecked(), tpl->GetFunction(context).ToLocalChecked());
  }

  void
  EnPhoneticDistance::New(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (args.IsConstructCall()) {
      speech::EnPhoneticDistance distance{};
      auto obj = new EnPhoneticDistance(std::move(distance));
      obj->Wrap(args.This());
      args.GetReturnValue().Set(args.This());
    } else {
      isolate->ThrowException(v8::Exception::SyntaxError(
        v8::String::NewFromUtf8(isolate, "Not invoked as constructor, change to: `new EnPhoneticDistance()`").ToLocalChecked()));
      return;
    }
  }

  void
  EnPhoneticDistance::Distance(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (args.Length() < 2) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected 2 arguments.").ToLocalChecked()));
      return;
    }

    auto enPronunciationType = EnPronunciation::type(isolate);
    if (!enPronunciationType->HasInstance(args[0]) || !enPronunciationType->HasInstance(args[1])) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected arguments to be EnPronunciation.").ToLocalChecked()));
      return;
    }

    auto obj = ObjectWrap::Unwrap<EnPhoneticDistance>(args.Holder());
    auto a = ObjectWrap::Unwrap<EnPronunciation>(args[0].As<v8::Object>());
    auto b = ObjectWrap::Unwrap<EnPronunciation>(args[1].As<v8::Object>());
    auto distance = obj->distance()(a->pronunciation(), b->pronunciation());

    args.GetReturnValue().Set(v8::Number::New(isolate, distance));
  }

  const speech::EnPhoneticDistance&
  EnPhoneticDistance::distance() const
  {
    return m_distance;
  }
}
}
}
