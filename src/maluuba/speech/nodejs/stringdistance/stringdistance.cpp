// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/nodejs/stringdistance.hpp"
#include <utility>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  v8::Persistent<v8::Function> StringDistance::s_constructor;
  v8::Persistent<v8::FunctionTemplate> StringDistance::s_type;

  StringDistance::StringDistance(LevenshteinDistance<> distance)
    : m_distance{std::move(distance)}
  { }

  StringDistance::~StringDistance() = default;

  v8::Local<v8::FunctionTemplate>
  StringDistance::type(v8::Isolate* isolate)
  {
    return s_type.Get(isolate);
  }

  void
  StringDistance::Init(v8::Local<v8::Object> exports)
  {
    auto isolate = exports->GetIsolate();

    auto tpl = v8::FunctionTemplate::New(isolate, New);
    tpl->SetClassName(v8::String::NewFromUtf8(isolate, "StringDistance"));
    tpl->InstanceTemplate()->SetInternalFieldCount(1);

    NODE_SET_PROTOTYPE_METHOD(tpl, "distance", Distance);

    s_constructor.Reset(isolate, tpl->GetFunction());
    s_type.Reset(isolate, tpl);
    exports->Set(v8::String::NewFromUtf8(isolate, "StringDistance"), tpl->GetFunction());
  }

  void
  StringDistance::New(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (args.IsConstructCall()) {
      LevenshteinDistance<> distance{};
      auto obj = new StringDistance(std::move(distance));
      obj->Wrap(args.This());
      args.GetReturnValue().Set(args.This());
    } else {
      isolate->ThrowException(v8::Exception::SyntaxError(
        v8::String::NewFromUtf8(isolate, "Not invoked as constructor, change to: `new StringDistance()`")));
      return;
    }
  }

  void
  StringDistance::Distance(const v8::FunctionCallbackInfo<v8::Value>& args)
  {
    auto isolate = args.GetIsolate();

    if (args.Length() < 2) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected 2 arguments.")));
      return;
    }

    if (!args[0]->IsString() || !args[1]->IsString()) {
      isolate->ThrowException(v8::Exception::TypeError(
          v8::String::NewFromUtf8(isolate, "Expected arguments to be string.")));
      return;
    }

    auto obj = ObjectWrap::Unwrap<StringDistance>(args.Holder());
    std::string a{*v8::String::Utf8Value{isolate, args[0]}};
    std::string b{*v8::String::Utf8Value{isolate, args[1]}};
    auto distance = obj->distance()(a, b);

    args.GetReturnValue().Set(v8::Number::New(isolate, distance));
  }

  const LevenshteinDistance<>&
  StringDistance::distance() const
  {
    return m_distance;
  }
}
}
}
