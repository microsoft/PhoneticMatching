// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/nodejs/match.hpp"
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
    getDistance(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Match>(info.Holder());
      auto distance = obj->match().distance();
      info.GetReturnValue().Set(v8::Number::New(isolate, distance));
    }

    void
    getElement(v8::Local<v8::String> property, const v8::PropertyCallbackInfo<v8::Value>& info)
    {
      auto isolate = info.GetIsolate();
      auto obj = node::ObjectWrap::Unwrap<nodejs::Match>(info.Holder());
      auto element = obj->match().element().Get(isolate);
      info.GetReturnValue().Set(element);
    }

    void
    setThrow(v8::Local<v8::String> property, v8::Local<v8::Value> value, const v8::PropertyCallbackInfo<void>& info)
    {
      auto isolate = info.GetIsolate();
      isolate->ThrowException(v8::Exception::Error(
          v8::String::NewFromUtf8(isolate, "Object is immutable, setters not allowed.", v8::NewStringType::kNormal).ToLocalChecked()));
      return;
    }
  }

  v8::Persistent<v8::Function> Match::s_constructor;

  Match::Match(Match::MatchType match)
    : m_match{std::move(match)}
  { }

  v8::Local<v8::Function>
  Match::constructor(v8::Isolate* isolate)
  {
    return v8::Local<v8::Function>::New(isolate, s_constructor);
  }

  void
  Match::Init(v8::Local<v8::Object> exports)
  {
    auto isolate = exports->GetIsolate();
    v8::Local<v8::Context> context = isolate->GetCurrentContext();

    auto tpl = v8::FunctionTemplate::New(isolate, New);
    tpl->SetClassName(v8::String::NewFromUtf8(isolate, "Match", v8::NewStringType::kNormal).ToLocalChecked());
    tpl->InstanceTemplate()->SetInternalFieldCount(1);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "distance", v8::NewStringType::kNormal).ToLocalChecked(), getDistance, setThrow);
    tpl->InstanceTemplate()->SetAccessor(v8::String::NewFromUtf8(isolate, "element", v8::NewStringType::kNormal).ToLocalChecked(), getElement, setThrow);

    s_constructor.Reset(isolate, tpl->GetFunction(context).ToLocalChecked());
  }

  void
  Match::New(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (!args[0]->IsExternal()) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Not Expected to initialize directly, use a Fuzzy Matcher.", v8::NewStringType::kNormal).ToLocalChecked()));
      return;
    }

    auto self = args.Holder();
    auto external = args[0].As<v8::External>();
    auto obj = static_cast<Match*>(external->Value());
    obj->Wrap(self);
    args.GetReturnValue().Set(self);
  }

  const Match::MatchType&
  Match::match() const
  {
    return m_match;
  }
}
}
}
