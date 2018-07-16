/**
 * @file
 * Fuzzy Matcher.
 *
 * @author Benedicte Pierrejean
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_FUZZYMATCHER_HPP
#define MALUUBA_SPEECH_FUZZYMATCHER_HPP

#include "maluuba/debug.hpp"
#include "maluuba/vptree.hpp"
#include "maluuba/xtd/optional.hpp"
#include <algorithm>
#include <functional>
#include <limits>
#include <utility>
#include <vector>

namespace maluuba
{
namespace speech
{
  /**
   * A fuzzy matcher.
   *
   * @tparam T  The type of element to store.
   */
  template <typename T>
  class FuzzyMatcher
  {
  public:
    /**
     * A match found by the fuzzy matcher.
     */
    class Match
    {
    public:
      Match(const T& element, double distance)
        : m_element{&element}, m_distance{distance}
      { }

      /**
       * @return The found element.
       */
      const T&
      element() const
      {
        return *m_element;
      }

      /**
       * @return The metric distance from the target to this element.
       */
      double
      distance() const
      {
        return m_distance;
      }

      friend bool
      operator<(const Match& l, const Match& r)
      {
        return l.m_distance < r.m_distance;
      }

    private:
      const T* m_element;
      double m_distance;
    };

    FuzzyMatcher() = default;
    virtual ~FuzzyMatcher() = default;

    virtual bool empty() const = 0;
    virtual size_t size() const = 0;

    FuzzyMatcher(FuzzyMatcher&& other) = default;
    FuzzyMatcher& operator=(FuzzyMatcher&& other) = default;
  };

  /**
   * A fuzzy matcher that compares the target to all the stored elements once.
   *
   * @tparam Target  The type of element to store.
   * @tparam DistanceMetric  The metric used to compare elements.
   */
  template <typename Target, typename DistanceMetric>
  class LinearFuzzyMatcher: public FuzzyMatcher<Target>
  {
    using Match = typename FuzzyMatcher<Target>::Match;

  public:
    LinearFuzzyMatcher() = default;

    template <typename Iterator>
    explicit LinearFuzzyMatcher(Iterator begin, Iterator end, DistanceMetric distance)
      : m_targets{begin, end},
        m_distance{std::move(distance)}
    { }

    virtual ~LinearFuzzyMatcher() = default;

    LinearFuzzyMatcher(LinearFuzzyMatcher&& other) = default;
    LinearFuzzyMatcher& operator=(LinearFuzzyMatcher&& other) = default;

    /**
     * @return true iff size == 0
     */
    virtual bool
    empty() const
    {
      return m_targets.empty();
    }

    /**
     * @return The number of targets constructed with.
     */
    virtual size_t
    size() const
    {
      return m_targets.size();
    }

    /**
     * Find the nearest element.
     *
     * @tparam T  To be compatible with @c DistanceMetric.
     * @param target  The search target.
     * @return The closest match to @p target, or @c nullopt if the matcher was empty.
     */
    template <typename T>
    xtd::optional<Match>
    find_nearest(const T& target) const
    {
      auto matches = find_k_nearest_within(target, 1, std::numeric_limits<double>::infinity());
      if (matches.empty()) {
        return xtd::nullopt;
      } else {
        return matches.front();
      }
    }

    /**
     * Find the nearest element.
     *
     * @tparam T  To be compatible with @c DistanceMetric.
     * @param target  The search target.
     * @param limit  The maximum distance to a match.
     * @return The closest match to @p target within @p limit, or @c nullopt if
     *         no match is found.
     */
    template <typename T>
    xtd::optional<Match>
    find_nearest_within(const T& target, double limit) const
    {
      auto matches = find_k_nearest_within(target, 1, limit);
      if (matches.empty()) {
        return xtd::nullopt;
      } else {
        return matches.front();
      }
    }

    /**
     * Find the @p k nearest elements.
     *
     * @tparam T  To be compatible with @c DistanceMetric.
     * @param target  The search target.
     * @param k  The maximum number of result to return.
     * @return The @p k nearest elements to @p target.
     */
    template <typename T>
    std::vector<Match>
    find_k_nearest(const T& target, size_t k) const
    {
      return find_k_nearest_within(target, k, std::numeric_limits<double>::infinity());
    }

    /**
     * Find the @p k nearest elements.
     *
     * @tparam T  To be compatible with @c DistanceMetric.
     * @param target  The search target.
     * @param k The maximum number of result to return.
     * @param limit  The maximum distance to a match.
     * @return The @p k nearest elements to @p target within @p limit.
     */
    template <typename T>
    std::vector<Match>
    find_k_nearest_within(const T& target, size_t k, double limit) const
    {
      check(k > 0, "k must be > 0");

      std::vector<Match> matches;
      for (const auto& possible_match: m_targets) {
        auto current = m_distance(possible_match, target);
        if (current <= limit) {
          if (matches.size() < k || current < matches.front().distance()) {
            if (matches.size() >= k) {
              std::pop_heap(matches.begin(), matches.end());
              matches.pop_back();
            }
            matches.emplace_back(possible_match, current);
            std::push_heap(matches.begin(), matches.end());
          }
        }
      }
      std::sort_heap(matches.begin(), matches.end());
      return matches;
    }

  private:
    std::vector<Target> m_targets;
    DistanceMetric m_distance;
  };

  /**
   * A fuzzy matcher that compares the target to a precomputed data structure of the stored elements
   * to minimize the number of comparisons.
   *
   * @tparam Target  The type of element to store.
   * @tparam DistanceMetric  The metric used to compare elements.
   */
  template <typename Target, typename DistanceMetric>
  class AcceleratedFuzzyMatcher: public FuzzyMatcher<Target>
  {
    using Match = typename FuzzyMatcher<Target>::Match;

  public:
    AcceleratedFuzzyMatcher() = default;

    template <typename Iterator>
    explicit AcceleratedFuzzyMatcher(Iterator begin, Iterator end, DistanceMetric distance)
      : m_vptree{begin, end, std::move(distance)}
    { }

    virtual ~AcceleratedFuzzyMatcher() = default;

    AcceleratedFuzzyMatcher(AcceleratedFuzzyMatcher&& other) = default;
    AcceleratedFuzzyMatcher& operator=(AcceleratedFuzzyMatcher&& other) = default;

    /**
     * @return true iff size == 0
     */
    virtual bool
    empty() const
    {
      return m_vptree.empty();
    }

    /**
     * @return The number of targets constructed with.
     */
    virtual size_t
    size() const
    {
      return m_vptree.size();
    }

    /**
     * Find the nearest element.
     *
     * @tparam T  To be compatible with @c DistanceMetric.
     * @param target  The search target.
     * @return The closest match to @p target, or @c nullopt if the matcher was empty.
     */
    template <typename T>
    xtd::optional<Match>
    find_nearest(const T& target) const
    {
      auto matches = find_k_nearest_within(target, 1, std::numeric_limits<double>::infinity());
      if (matches.empty()) {
        return xtd::nullopt;
      } else {
        return matches.front();
      }
    }

    /**
     * Find the nearest element.
     *
     * @tparam T  To be compatible with @c DistanceMetric.
     * @param target  The search target.
     * @param limit  The maximum distance to a match.
     * @return The closest match to @p target within @p limit, or @c nullopt if
     *         no match is found.
     */
    template <typename T>
    xtd::optional<Match>
    find_nearest_within(const T& target, double limit) const
    {
      auto matches = find_k_nearest_within(target, 1, limit);
      if (matches.empty()) {
        return xtd::nullopt;
      } else {
        return matches.front();
      }
    }

    /**
     * Find the @p k nearest elements.
     *
     * @tparam T  To be compatible with @c DistanceMetric.
     * @param target  The search target.
     * @param k  The maximum number of result to return.
     * @return The @p k nearest elements to @p target.
     */
    template <typename T>
    std::vector<Match>
    find_k_nearest(const T& target, size_t k) const
    {
      return find_k_nearest_within(target, k, std::numeric_limits<double>::infinity());
    }

    /**
     * Find the @p k nearest elements.
     *
     * @tparam T  To be compatible with @c DistanceMetric.
     * @param target  The search target.
     * @param k The maximum number of result to return.
     * @param limit  The maximum distance to a match.
     * @return The @p k nearest elements to @p target within @p limit.
     */
    template <typename T>
    std::vector<Match>
    find_k_nearest_within(const T& target, size_t k, double limit) const
    {
      check(k > 0, "k must be > 0");

      auto matches = m_vptree.find_k_nearest_within(target, k, limit);
      std::vector<Match> results;
      for (const auto& match: matches) {
        results.emplace_back(match.element(), match.distance());
      }
      return results;
    }

  private:
    VpTree<Target, DistanceMetric> m_vptree;
  };
}
}

#endif // MALUUBA_SPEECH_FUZZYMATCHER_HPP
