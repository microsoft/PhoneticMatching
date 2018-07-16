// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/nodejs/performance.hpp"

namespace maluuba
{
namespace speech
{
namespace nodejs
{
    v8::Persistent<v8::Object> Performance::s_performance;

    void
    Performance::Init(v8::Local<v8::Object> module)
    {
      auto isolate = module->GetIsolate();
      auto require = module->Get(v8::String::NewFromUtf8(isolate, "require")).As<v8::Function>();
      const auto argc = 1;
      v8::Local<v8::Value> argv[argc] = { v8::String::NewFromUtf8(isolate, "perf_hooks") };
      auto perf_hooks = require->Call(module, argc, argv).As<v8::Object>();
      auto performance = perf_hooks->Get(v8::String::NewFromUtf8(isolate, "performance")).As<v8::Object>();
      s_performance.Reset(isolate, performance);
    }

    void
    Performance::Mark(const std::string& name)
    {
      auto isolate = v8::Isolate::GetCurrent();
      auto performance = s_performance.Get(isolate);
      auto mark = performance->Get(v8::String::NewFromUtf8(isolate, "mark")).As<v8::Function>();
      const auto argc = 1;
      v8::Local<v8::Value> argv[argc] = { v8::String::NewFromUtf8(isolate, name.data()) };
      mark->Call(performance, argc, argv);
    }

    void
    Performance::Measure(const std::string& name, const std::string& start_mark, const std::string& end_mark)
    {
      auto isolate = v8::Isolate::GetCurrent();
      auto performance = s_performance.Get(isolate);
      auto measure = performance->Get(v8::String::NewFromUtf8(isolate, "measure")).As<v8::Function>();
      const auto argc = 3;
      v8::Local<v8::Value> argv[argc] = { v8::String::NewFromUtf8(isolate, name.data()),
         v8::String::NewFromUtf8(isolate, start_mark.data()), v8::String::NewFromUtf8(isolate, end_mark.data()) };
      measure->Call(performance, argc, argv);
    }
}
}
}
