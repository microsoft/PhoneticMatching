/**
 * @file
 * Vantage point trees.
 *
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_VPTREE_HPP
#define MALUUBA_VPTREE_HPP

#include "maluuba/metric.hpp"
#include "maluuba/xtd/optional.hpp"
#include <algorithm>
#include <initializer_list>
#include <queue>

namespace maluuba
{
  /**
   * A vantage point tree.
   *
   * @tparam T  The type of element to store.
   * @tparam Metric  The metric used to compare elements.
   * @author Tavian Barnes (tavian.barnes@microsoft.com)
   */
  template <typename T, typename Metric>
  class VpTree
  {
  public:
    using value_type = T;
    using distance_type = MetricResult<Metric, T>;

  private:
    struct Node
    {
      T element;
      distance_type radius;
      std::size_t left_size;

      Node(T element)
        : element(std::move(element)), radius{}, left_size{}
      { }
    };

    using NodeVector = std::vector<Node>;
    using NodeIterator = typename NodeVector::const_iterator;

  public:
    using size_type = typename NodeVector::size_type;
    using difference_type = typename NodeVector::difference_type;

    using reference = value_type&;
    using const_reference = const value_type&;

    using pointer = value_type*;
    using const_pointer = const value_type*;

    VpTree() = default;

    explicit VpTree(Metric metric)
      : m_metric{std::move(metric)}
    { }

    explicit VpTree(std::initializer_list<T> ilist, Metric metric = Metric{})
      : m_nodes{ilist.begin(), ilist.end()},
        m_metric{std::move(metric)}
    {
      build_tree();
    }

    template <typename Iterator>
    explicit VpTree(Iterator first, Iterator last, Metric metric = Metric{})
      : m_nodes{first, last},
        m_metric{std::move(metric)}
    {
      build_tree();
    }

    bool
    empty() const
    {
      return m_nodes.empty();
    }

    size_type
    size() const
    {
      return m_nodes.size();
    }

    /**
     * A near match found in the tree.
     */
    class Match
    {
    public:
      Match() = default;

      Match(NodeIterator node, distance_type distance)
        : m_node{node}, m_distance{distance}
      { }

      /**
       * @return The found element.
       */
      const T&
      element() const
      {
        return m_node->element;
      }

      /**
       * @return The metric distance from the target to this element.
       */
      distance_type
      distance() const
      {
        return m_distance;
      }

    private:
      friend bool
      operator<(const Match& lhs, const Match& rhs)
      {
        return lhs.distance() < rhs.distance();
      }

      NodeIterator m_node;
      distance_type m_distance;
    };

  private:
    /**
     * An entry in the search stack.
     */
    struct StackEntry
    {
      /** The range to search. */
      NodeIterator first, last;
      /** Search is necessary iff a <= b + tau. */
      distance_type a, b;

      StackEntry(NodeIterator first, NodeIterator last, distance_type a, distance_type b)
        : first{first}, last{last}, a{a}, b{b}
      { }
    };

    using SearchStack = std::vector<StackEntry>;

  public:
    /**
     * Find the nearest element in the tree.
     *
     * @param target  The search target.
     * @return The closest match to @p target, or @c nullopt if the tree is
     *         empty.
     */
    template <typename U>
    xtd::optional<Match>
    find_nearest(const U& target) const
    {
      auto matches = find_k_nearest(target, 1);
      if (matches.empty()) {
        return xtd::nullopt;
      } else {
        return matches[0];
      }
    }

    /**
     * Find the nearest element in the tree.
     *
     * @param target  The search target.
     * @param limit  The maximum distance to a match.
     * @return The closest match to @p target within @p limit, or @c nullopt if
     *         no match is found.
     */
    template <typename U>
    xtd::optional<Match>
    find_nearest_within(const U& target, distance_type limit) const
    {
      auto matches = find_k_nearest_within(target, 1, limit);
      if (matches.empty()) {
        return xtd::nullopt;
      } else {
        return matches[0];
      }
    }

    /**
     * Find the @p k nearest elements in the tree.
     *
     * @param target  The search target.
     * @param k  The maximum number of result to return.
     * @return The @p k nearest elements in the tree to @p target.
     */
    template <typename U>
    std::vector<Match>
    find_k_nearest(const U& target, size_type k) const
    {
      std::priority_queue<Match> matches;
      distance_type tau{};

      SearchStack stack;
      stack.emplace_back(m_nodes.begin(), m_nodes.end(), 0, 0);

      while (!stack.empty()) {
        auto entry = stack.back();
        stack.pop_back();

        if (entry.first == entry.last || (matches.size() == k && entry.a > entry.b + tau)) {
          continue;
        }

        auto root = entry.first;
        auto distance = m_metric(root->element, target);
        if (matches.size() < k || distance <= tau) {
          if (matches.size() == k) {
            matches.pop();
          }
          matches.push(Match(root, distance));
          tau = matches.top().distance();
        }

        auto left = root + 1;
        auto right = entry.last;
        if (left == right) {
          continue;
        }

        auto mid = left + root->left_size;

        auto radius = root->radius;

        if (distance < radius) {
          stack.emplace_back(mid, right, radius, distance);
          stack.emplace_back(left, mid, distance, radius);
        } else {
          stack.emplace_back(left, mid, distance, radius);
          stack.emplace_back(mid, right, radius, distance);
        }
      }

      auto i = matches.size();
      std::vector<Match> result{i};
      while (!matches.empty()) {
        result[--i] = matches.top();
        matches.pop();
      }
      return result;
    }

    /**
     * Find the @p k nearest elements in the tree.
     *
     * @param target  The search target.
     * @param k  The maximum number of result to return.
     * @param limit  The maximum distance to a match.
     * @return The @p k nearest elements in the tree to @p target within @p limit.
     */
    template <typename U>
    std::vector<Match>
    find_k_nearest_within(const U& target, size_type k, distance_type limit) const
    {
      std::priority_queue<Match> matches;
      distance_type tau = limit;

      SearchStack stack;
      stack.emplace_back(m_nodes.begin(), m_nodes.end(), 0, 0);

      while (!stack.empty()) {
        auto entry = stack.back();
        stack.pop_back();

        if (entry.first == entry.last || entry.a > entry.b + tau) {
          continue;
        }

        auto root = entry.first;
        auto distance = m_metric(root->element, target);
        if (distance <= tau) {
          if (matches.size() == k) {
            matches.pop();
          }
          matches.push(Match(root, distance));
          if (matches.size() == k) {
            tau = matches.top().distance();
          }
        }

        auto left = root + 1;
        auto right = entry.last;
        if (left == right) {
          continue;
        }

        auto mid = left + root->left_size;

        auto radius = root->radius;

        if (distance < radius) {
          stack.emplace_back(mid, right, radius, distance);
          stack.emplace_back(left, mid, distance, radius);
        } else {
          stack.emplace_back(left, mid, distance, radius);
          stack.emplace_back(mid, right, radius, distance);
        }
      }

      auto i = matches.size();
      std::vector<Match> result{i};
      while (!matches.empty()) {
        result[--i] = matches.top();
        matches.pop();
      }
      return result;
    }

  private:
    NodeVector m_nodes;
    Metric m_metric;

    void
    build_tree()
    {
      using iterator = typename NodeVector::iterator;
      using SubRange = std::pair<iterator, iterator>;

      std::vector<SubRange> stack;
      stack.emplace_back(m_nodes.begin(), m_nodes.end());

      while (!stack.empty()) {
        auto range = stack.back();
        stack.pop_back();

        if (range.second - range.first <= 1) {
          continue;
        }

        auto root = range.first;
        auto begin = root + 1;
        auto end = range.second;
        auto mid = begin + (end - begin)/2;

        auto compare = [=] (const Node& a, const Node& b) {
          return m_metric(root->element, a.element) < m_metric(root->element, b.element);
        };
        std::nth_element(begin, mid, end, compare);

        root->radius = m_metric(root->element, mid->element);
        root->left_size = mid - begin;
        stack.emplace_back(mid, end);
        stack.emplace_back(begin, mid);
      }
    }
  };
}

#endif // MALUUBA_VPTREE_HPP
