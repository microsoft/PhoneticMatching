/**
 * @file
 * Fuzzy Matcher wrapped in NodeJS. Both normal and accelerated versions.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_NODEJS_FUZZYMATCHER_HPP
#define MALUUBA_SPEECH_NODEJS_FUZZYMATCHER_HPP

#include "maluuba/speech/nodejs/enphoneticdistance.hpp"
#include "maluuba/speech/nodejs/match.hpp"
#include "maluuba/speech/nodejs/stringdistance.hpp"
#include "maluuba/speech/fuzzymatcher.hpp"
#include "maluuba/speech/pronouncer.hpp"
#include "maluuba/debug.hpp"
#include "maluuba/xtd/optional.hpp"
#include <node.h>
#include <node_object_wrap.h>
#include <functional>
#include <iterator>
#include <utility>

namespace maluuba
{
namespace speech
{
namespace nodejs
{
  namespace
  {
    using NodeJsTarget = v8::UniquePersistent<v8::Value>;

    struct Target
    {
      NodeJsTarget target;
      xtd::optional<NodeJsTarget> extraction;
      xtd::optional<std::string> phrase;
      xtd::optional<speech::EnPronunciation> pronunciation;

      Target(NodeJsTarget target, NodeJsTarget extraction)
        : target{std::move(target)}, extraction{std::move(extraction)}
      { }

      Target(NodeJsTarget target, std::string phrase)
        : target{std::move(target)}, phrase{std::move(phrase)}
      { }

      Target(NodeJsTarget target, speech::EnPronunciation pronunciation)
        : target{std::move(target)}, pronunciation{std::move(pronunciation)}
      { }

      Target(NodeJsTarget target, std::string phrase, speech::EnPronunciation pronunciation)
        : target{std::move(target)}, phrase{std::move(phrase)}, pronunciation{std::move(pronunciation)}
      { }

    };
  }

  template <template <typename, typename> typename MatcherType>
  class FuzzyMatcher: public node::ObjectWrap
  {
    using NodeDistanceMetric = std::function<double(const Target&,const Target&)>;
    using Matcher = MatcherType<Target, NodeDistanceMetric>;

  public:
    static void Init(v8::Local<v8::Object> exports, const xtd::string_view className)
    {
      auto isolate = exports->GetIsolate();

      auto localClassName = v8::String::NewFromUtf8(isolate, className.data(), v8::String::kNormalString, className.length());
      auto tpl = v8::FunctionTemplate::New(isolate, New);
      tpl->SetClassName(localClassName);
      tpl->InstanceTemplate()->SetInternalFieldCount(1);

      NODE_SET_PROTOTYPE_METHOD(tpl, "empty", Empty);
      NODE_SET_PROTOTYPE_METHOD(tpl, "size", Size);
      NODE_SET_PROTOTYPE_METHOD(tpl, "nearest", Nearest);
      NODE_SET_PROTOTYPE_METHOD(tpl, "nearestWithin", NearestWithin);
      NODE_SET_PROTOTYPE_METHOD(tpl, "kNearest", KNearest);
      NODE_SET_PROTOTYPE_METHOD(tpl, "kNearestWithin", KNearestWithin);

      s_constructor.Reset(isolate, tpl->GetFunction());
      exports->Set(localClassName, tpl->GetFunction());
    }

    const Matcher& matcher() const
    {
      return m_matcher;
    }

    Target to_target(v8::Isolate* isolate, v8::Local<v8::Value> arg, double& threshold_scale) const
    {
      return m_to_target(isolate, arg, threshold_scale);
    }

  private:
    template <typename Targets>
    explicit FuzzyMatcher(Targets targets, NodeDistanceMetric metric, std::function<Target(v8::Isolate*, v8::Local<v8::Value>, double&)> to_target)
      : m_matcher{std::make_move_iterator(targets.begin()), std::make_move_iterator(targets.end()), std::move(metric)},
        m_to_target{to_target}
    { }

    static FuzzyMatcher<MatcherType>*
    make_fuzzy_matcher_hybrid(v8::Isolate* isolate, v8::Local<v8::Array> arg_targets, v8::Local<v8::Value> arg_distance, v8::Local<v8::Function> arg_extract)
    {
      speech::EnPronouncer pronouncer{};
      std::vector<Target> targets;
      const auto argc = 1;
      for (uint32_t i = 0; i < arg_targets->Length(); ++i) {
        auto obj = arg_targets->Get(i);
        auto value = obj;
        if (!arg_extract.IsEmpty()) {
          v8::Local<v8::Value> argv[argc] = { obj };
          value = arg_extract->Call(v8::Null(isolate), argc, argv);
        }
        std::string phrase{*v8::String::Utf8Value{isolate, value}};
        targets.emplace_back(NodeJsTarget(isolate, obj), phrase, std::move(pronouncer.pronounce(phrase)));
      }

      // copy out the native distance component.
      auto obj = ObjectWrap::Unwrap<EnHybridDistance>(arg_distance.As<v8::Object>());
      auto metric = [distance{obj->distance()}](const auto& a, const auto& b) {
        return distance(*a.phrase, *a.pronunciation, *b.phrase, *b.pronunciation);
      };

      auto to_target = [phonetic_weight_percentage{obj->distance().phonetic_weight_percentage()}](auto isolate, auto arg, auto& threshold_scale) {
        static speech::EnPronouncer pronouncer{};
        std::string phrase{*v8::String::Utf8Value{isolate, arg}};
        auto pronunciation = pronouncer.pronounce(phrase);

        threshold_scale = phonetic_weight_percentage * pronunciation.size() + (1-phonetic_weight_percentage) * phrase.length();
        if (threshold_scale == 0) threshold_scale = 1;
        Target target(NodeJsTarget{}, phrase, std::move(pronunciation));
        return target;
      };

      return new FuzzyMatcher(std::move(targets), NodeDistanceMetric{std::move(metric)}, std::move(to_target));
    }

    static FuzzyMatcher<MatcherType>*
    make_fuzzy_matcher_string(v8::Isolate* isolate, v8::Local<v8::Array> arg_targets, v8::Local<v8::Value> arg_distance, v8::Local<v8::Function> arg_extract)
    {
      std::vector<Target> targets;
      const auto argc = 1;
      for (uint32_t i = 0; i < arg_targets->Length(); ++i) {
        auto obj = arg_targets->Get(i);
        auto value = obj;
        if (!arg_extract.IsEmpty()) {
          v8::Local<v8::Value> argv[argc] = { obj };
          value = arg_extract->Call(v8::Null(isolate), argc, argv);
        }
        std::string phrase{*v8::String::Utf8Value{isolate, value}};
        targets.emplace_back(NodeJsTarget(isolate, obj), std::move(phrase));
      }

      // copy out the native distance component.
      auto obj = ObjectWrap::Unwrap<StringDistance>(arg_distance.As<v8::Object>());
      auto metric = [distance{obj->distance()}](const auto& a, const auto& b) {
        return distance(*a.phrase, *b.phrase);
      };

      auto to_target = [](auto isolate, auto arg, auto& threshold_scale) {
        std::string phrase{*v8::String::Utf8Value{isolate, arg}};
        threshold_scale = phrase.length() > 0 ? phrase.length() : 1;
        Target target(NodeJsTarget{}, std::move(phrase));
        return target;
      };

      return new FuzzyMatcher(std::move(targets), NodeDistanceMetric{std::move(metric)}, std::move(to_target));
    }

    static FuzzyMatcher<MatcherType>*
    make_fuzzy_matcher_phone(v8::Isolate* isolate, v8::Local<v8::Array> arg_targets, v8::Local<v8::Value> arg_distance, v8::Local<v8::Function> arg_extract)
    {
      speech::EnPronouncer pronouncer{};
      std::vector<Target> targets;
      const auto argc = 1;
      for (uint32_t i = 0; i < arg_targets->Length(); ++i) {
        auto obj = arg_targets->Get(i);
        auto value = obj;
        if (!arg_extract.IsEmpty()) {
          v8::Local<v8::Value> argv[argc] = { obj };
          value = arg_extract->Call(v8::Null(isolate), argc, argv);
        }
        std::string phrase{*v8::String::Utf8Value{isolate, value}};
        targets.emplace_back(NodeJsTarget(isolate, obj), std::move(pronouncer.pronounce(phrase)));
      }

      // copy out the native distance component.
      auto obj = ObjectWrap::Unwrap<EnPhoneticDistance>(arg_distance.As<v8::Object>());
      auto metric = [distance{obj->distance()}](const auto& a, const auto& b) {
        return distance(*a.pronunciation, *b.pronunciation);
      };

      auto to_target = [](auto isolate, auto arg, auto& threshold_scale) {
        static speech::EnPronouncer pronouncer{};
        std::string phrase{*v8::String::Utf8Value{isolate, arg}};
        auto pronunciation = pronouncer.pronounce(phrase);
        threshold_scale = pronunciation.size() > 0 ? pronunciation.size() : 1;
        Target target(NodeJsTarget{}, std::move(pronunciation));
        return target;
      };

      return new FuzzyMatcher(std::move(targets), NodeDistanceMetric{std::move(metric)}, std::move(to_target));
    }

    static FuzzyMatcher<MatcherType>*
    make_fuzzy_matcher_js(v8::Isolate* isolate, v8::Local<v8::Array> arg_targets, v8::Local<v8::Value> arg_distance, v8::Local<v8::Function> arg_extract)
    {
      std::vector<Target> targets;
      const auto argc = 1;
      for (uint32_t i = 0; i < arg_targets->Length(); ++i) {
        auto obj = arg_targets->Get(i);
        auto value = obj;
        if (!arg_extract.IsEmpty()) {
          v8::Local<v8::Value> argv[argc] = { obj };
          value = arg_extract->Call(v8::Null(isolate), argc, argv);
        }
        targets.emplace_back(NodeJsTarget(isolate, obj), NodeJsTarget(isolate, value));
      }

      // Need persistent reference to the user's distance function that needs to be copyable.
      v8::Persistent<v8::Function, v8::CopyablePersistentTraits<v8::Function>> distance(isolate, arg_distance.As<v8::Function>());
      auto metric = [distance{std::move(distance)}](const auto& a, const auto& b) {
        auto isolate = v8::Isolate::GetCurrent();
        const unsigned argc = 2;
        v8::Local<v8::Value> argv[argc] = { a.extraction->Get(isolate), b.extraction->Get(isolate) };
        auto value = distance.Get(isolate)->Call(v8::Null(isolate), argc, argv);
        check_logic(!value.IsEmpty() && value->IsNumber(), "Expected callback to return a number.");
        return value->NumberValue();
      };

      auto to_target = [](auto isolate, auto arg, auto& threshold_scale) {
        threshold_scale = 1;
        Target target(NodeJsTarget{}, NodeJsTarget(isolate, arg));
        return target;
      };

      return new FuzzyMatcher(std::move(targets), NodeDistanceMetric{std::move(metric)}, std::move(to_target));
    }

    ~FuzzyMatcher() = default;

    static void New(const v8::FunctionCallbackInfo<v8::Value>& args)
    {
      auto isolate = args.GetIsolate();

      if (args.IsConstructCall()) {
        if (args.Length() < 2) {
          isolate->ThrowException(v8::Exception::TypeError(
              v8::String::NewFromUtf8(isolate, "Expected at least 2 arguments.")));
          return;
        }

        if (!args[0]->IsArray()) {
          isolate->ThrowException(v8::Exception::TypeError(
              v8::String::NewFromUtf8(isolate, "Expected 'targets' argument to be an Object[].")));
          return;
        }
        v8::Local<v8::Function> arg_extract{};
        if (args.Length() > 2) {
          if (!args[2]->IsFunction()) {
            isolate->ThrowException(v8::Exception::TypeError(
                v8::String::NewFromUtf8(isolate, "Expected 'extract' argument to be a Function.")));
            return;
          }
          arg_extract = args[2].As<v8::Function>();
        }
        auto arg_targets = args[0].As<v8::Array>();
        auto arg_distance = args[1];

        try {
          // Attempt to see if the JS calls can be unwrapped into their native components
          // to save on overhead on the distance calls, which can occur a lot.
          if (EnHybridDistance::type(isolate)->HasInstance(arg_distance)) {
            auto obj = make_fuzzy_matcher_hybrid(isolate, arg_targets, arg_distance, arg_extract);
            obj->Wrap(args.This());
          } else if (StringDistance::type(isolate)->HasInstance(arg_distance)) {
            auto obj = make_fuzzy_matcher_string(isolate, arg_targets, arg_distance, arg_extract);
            obj->Wrap(args.This());
          } else if (EnPhoneticDistance::type(isolate)->HasInstance(arg_distance)) {
            auto obj = make_fuzzy_matcher_phone(isolate, arg_targets, arg_distance, arg_extract);
            obj->Wrap(args.This());
          } else {
            // User provided JS distance function.
            if (!arg_distance->IsFunction()) {
              isolate->ThrowException(v8::Exception::TypeError(
                  v8::String::NewFromUtf8(isolate, "Expected 'distance' argument to be a Function.")));
              return;
            }
            auto obj = make_fuzzy_matcher_js(isolate, arg_targets, arg_distance, arg_extract);
            obj->Wrap(args.This());
          }

          args.GetReturnValue().Set(args.This());
        } catch (const std::exception& e) {
          isolate->ThrowException(v8::Exception::TypeError(
              v8::String::NewFromUtf8(isolate, e.what())));
          return;
        }
      } else {
        isolate->ThrowException(v8::Exception::SyntaxError(
          v8::String::NewFromUtf8(isolate, "Not invoked as constructor, use `new`.")));
        return;
      }
    }

    static void Empty(const v8::FunctionCallbackInfo<v8::Value>& args)
    {
      auto isolate = args.GetIsolate();

      auto obj = ObjectWrap::Unwrap<FuzzyMatcher>(args.Holder());
      auto empty = obj->matcher().empty();
      args.GetReturnValue().Set(v8::Boolean::New(isolate, empty));
    }

    static void Size(const v8::FunctionCallbackInfo<v8::Value>& args)
    {
      auto isolate = args.GetIsolate();

      auto obj = ObjectWrap::Unwrap<FuzzyMatcher>(args.Holder());
      auto size = obj->matcher().size();
      args.GetReturnValue().Set(v8::Number::New(isolate, size));
    }

    static void Nearest(const v8::FunctionCallbackInfo<v8::Value>& args)
    {
      auto isolate = args.GetIsolate();

      if (args.Length() < 1) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected 1 argument.")));
        return;
      }

      auto obj = ObjectWrap::Unwrap<FuzzyMatcher>(args.Holder());
      double threshold_scale;
      auto target = obj->to_target(isolate, args[0], threshold_scale);

      try {
        auto match = obj->matcher().find_nearest(target);

        if (match) {
          speech::FuzzyMatcher<NodeJsTarget>::Match m(match->element().target, match->distance() / threshold_scale);
          auto wrap_match = new Match(std::move(m));
          auto context = isolate->GetCurrentContext();
          const auto argc = 1;
          v8::Local<v8::Value> argv[argc] = { v8::External::New(isolate, wrap_match) };
          auto instance = Match::constructor(isolate)->NewInstance(context, argc, argv).ToLocalChecked();
          args.GetReturnValue().Set(instance);
        } else {
          args.GetReturnValue().Set(v8::Undefined(isolate));
        }
      } catch(const std::exception& e) {
        isolate->ThrowException(v8::Exception::Error(
            v8::String::NewFromUtf8(isolate, e.what())));
        return;
      }
    }

    static void NearestWithin(const v8::FunctionCallbackInfo<v8::Value>& args)
    {
      auto isolate = args.GetIsolate();

      if (args.Length() < 2) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected 2 arguments.")));
        return;
      }

      if (!args[1]->IsNumber()) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected argument to be a number.")));
        return;
      }

      auto obj = ObjectWrap::Unwrap<FuzzyMatcher>(args.Holder());
      double threshold_scale;
      auto target = obj->to_target(isolate, args[0], threshold_scale);
      auto threshold = args[1]->NumberValue() * threshold_scale;

      try {
        auto match = obj->matcher().find_nearest_within(target, threshold);

        if (match) {
          speech::FuzzyMatcher<NodeJsTarget>::Match m(match->element().target, match->distance() / threshold_scale);
          auto wrap_match = new Match(std::move(m));
          auto context = isolate->GetCurrentContext();
          const auto argc = 1;
          v8::Local<v8::Value> argv[argc] = { v8::External::New(isolate, wrap_match) };
          auto instance = Match::constructor(isolate)->NewInstance(context, argc, argv).ToLocalChecked();
          args.GetReturnValue().Set(instance);
        } else {
          args.GetReturnValue().Set(v8::Undefined(isolate));
        }
      } catch(const std::exception& e) {
        isolate->ThrowException(v8::Exception::Error(
            v8::String::NewFromUtf8(isolate, e.what())));
        return;
      }
    }

    static void KNearest(const v8::FunctionCallbackInfo<v8::Value>& args)
    {
      auto isolate = args.GetIsolate();

      if (args.Length() < 2) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected 2 arguments.")));
        return;
      }

      if (!args[1]->IsUint32()) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected argument to be an integer.")));
        return;
      }

      auto obj = ObjectWrap::Unwrap<FuzzyMatcher>(args.Holder());
      double threshold_scale;
      auto target = obj->to_target(isolate, args[0], threshold_scale);
      auto k = args[1]->Uint32Value();

      try {
        auto matches = obj->matcher().find_k_nearest(target, k);

        auto context = isolate->GetCurrentContext();
        auto wrap_matches = v8::Array::New(isolate, matches.size());
        for (size_t i = 0; i < matches.size(); ++i) {
          speech::FuzzyMatcher<NodeJsTarget>::Match match(matches[i].element().target, matches[i].distance() / threshold_scale);
          auto wrap_match = new Match(std::move(match));

          const auto argc = 1;
          v8::Local<v8::Value> argv[argc] = { v8::External::New(isolate, wrap_match) };
          auto instance = Match::constructor(isolate)->NewInstance(context, argc, argv).ToLocalChecked();
          wrap_matches->Set(i, instance);
        }
        args.GetReturnValue().Set(wrap_matches);
      } catch(const std::exception& e) {
        isolate->ThrowException(v8::Exception::Error(
            v8::String::NewFromUtf8(isolate, e.what())));
        return;
      }
    }

    static void KNearestWithin(const v8::FunctionCallbackInfo<v8::Value>& args)
    {
      auto isolate = args.GetIsolate();

      if (args.Length() < 3) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected 3 arguments.")));
        return;
      }

      if (!args[1]->IsUint32()) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected argument to be an integer.")));
        return;
      }
      if (!args[2]->IsNumber()) {
        isolate->ThrowException(v8::Exception::TypeError(
            v8::String::NewFromUtf8(isolate, "Expected argument to be a number.")));
        return;
      }

      auto obj = ObjectWrap::Unwrap<FuzzyMatcher>(args.Holder());
      double threshold_scale;
      auto target = obj->to_target(isolate, args[0], threshold_scale);
      auto k = args[1]->Uint32Value();
      auto threshold = args[2]->NumberValue() * threshold_scale;

      try {
        auto matches = obj->matcher().find_k_nearest_within(target, k, threshold);

        auto context = isolate->GetCurrentContext();
        auto wrap_matches = v8::Array::New(isolate, matches.size());
        for (size_t i = 0; i < matches.size(); ++i) {
          speech::FuzzyMatcher<NodeJsTarget>::Match match(matches[i].element().target, matches[i].distance() / threshold_scale);
          auto wrap_match = new Match(std::move(match));

          const auto argc = 1;
          v8::Local<v8::Value> argv[argc] = { v8::External::New(isolate, wrap_match) };
          auto instance = Match::constructor(isolate)->NewInstance(context, argc, argv).ToLocalChecked();
          wrap_matches->Set(i, instance);
        }
        args.GetReturnValue().Set(wrap_matches);
      } catch(const std::exception& e) {
        isolate->ThrowException(v8::Exception::Error(
            v8::String::NewFromUtf8(isolate, e.what())));
        return;
      }
    }

    static v8::Persistent<v8::Function> s_constructor;
    Matcher m_matcher;
    std::function<Target(v8::Isolate*, v8::Local<v8::Value>, double&)> m_to_target;
  };

  template <template <typename, typename> typename T>
  v8::Persistent<v8::Function> FuzzyMatcher<T>::s_constructor;
}
}
}

#endif // MALUUBA_SPEECH_NODEJS_FUZZYMATCHER_HPP
