/**
 * @file
 * Pronouncer.
 *
 * @author Benedicte Pierrejean
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_PRONOUNCER_HPP
#define MALUUBA_SPEECH_PRONOUNCER_HPP

#include "maluuba/speech/pronunciation.hpp"
#include <memory>
#include <string>

namespace maluuba
{
namespace speech
{
  class Pronouncer
  {
  public:
    Pronouncer() = default;
    virtual ~Pronouncer() = 0;

    Pronouncer(Pronouncer&& other) = default;
    Pronouncer& operator=(Pronouncer&& other) = default;
  };

  class EnPronouncer: public Pronouncer
  {
  public:
    EnPronouncer();
    virtual ~EnPronouncer();

    EnPronouncer(EnPronouncer&& other);
    EnPronouncer& operator=(EnPronouncer&& other);

    EnPronunciation pronounce(const std::string& text) const;

  private:
    struct Impl;
    std::unique_ptr<Impl> m_impl;
  };
}
}

#endif // MALUUBA_SPEECH_PRONOUNCER_HPP
