/**
 * @file
 * Performance utility to make trace events.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_NODEJS_PERFORMANCE_HPP
#define MALUUBA_SPEECH_NODEJS_PERFORMANCE_HPP

#include <node.h>
#include <string>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  class Performance
  {
  public:
    static void Init(v8::Local<v8::Object> module);

    static void Mark(const std::string& name);
    static void Measure(const std::string& name, const std::string& start_mark, const std::string& end_mark);

  private:
    static v8::Persistent<v8::Object> s_performance;
  };
}
}
}

#endif // MALUUBA_SPEECH_NODEJS_PERFORMANCE_HPP
