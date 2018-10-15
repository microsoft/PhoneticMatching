// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "maluuba/speech/pronouncer.hpp"
#include <flite/lang/cmulex/cmu_lex.h>
#include <flite/lang/usenglish/usenglish.h>
#include <flite.h>
#include <memory>

namespace maluuba
{
namespace speech
{
  namespace
  {
    cst_utterance*
    no_wave_synth(cst_utterance* u)
    {
      return u;
    }

    cst_voice*
    no_wave_voice()
    {
      flite_init();

      cst_voice* v = new_voice();
      cst_lexicon* lex;

      v->name = "no_wave_voice";

      // Set up basic values for synthesizing with this voice
      usenglish_init(v);
      feat_set_string(v->features, "name", "cmu_us_no_wave");

      // Lexicon
      lex = cmu_lex_init();
      feat_set(v->features, "lexicon", lexicon_val(lex));

      // Post lexical rules
      feat_set(v->features, "postlex_func", uttfunc_val(lex->postlex));

      // Waveform synthesis: diphone_synth
      feat_set(v->features, "wave_synth_func", uttfunc_val(&no_wave_synth));

      return v;
    }
  }

  Pronouncer::~Pronouncer() = default;

  using VoiceHandle = std::unique_ptr<cst_voice, decltype(delete_voice)*>;

  struct EnPronouncer::Impl
  {
    VoiceHandle voice;

    Impl()
      : voice{no_wave_voice(), delete_voice}
    { }
  };

  EnPronouncer::EnPronouncer()
    : m_impl{std::make_unique<Impl>()}
  { }

  EnPronouncer::~EnPronouncer() = default;

  EnPronouncer::EnPronouncer(EnPronouncer&& other) = default;

  EnPronouncer&
  EnPronouncer::operator=(EnPronouncer&& other) = default;

  EnPronunciation
  EnPronouncer::pronounce(const std::string& text) const
  {
    using UtteranceHandle = std::unique_ptr<cst_utterance, decltype(delete_utterance)*>;

    std::vector<std::string> phonemes;

    auto utt = flite_synth_text(text.c_str(), m_impl->voice.get());
    UtteranceHandle utt_handle{utt, delete_utterance};

    for (auto s = relation_head(utt_relation(utt, "Segment")); s; s = item_next(s)) {
      std::string name = item_feat_string(s, "name");
      if (name == "pau") {
        continue;
      }

      if (strcmp("+", ffeature_string(s, "ph_vc")) == 0) {
        // If the phoneme is a vowel, add stress value
        name += ffeature_string(s, "R:SylStructure.parent.stress");
      }
      phonemes.push_back(std::move(name));
    }

    return EnPronunciation::from_arpabet(phonemes);
  }
}
}
