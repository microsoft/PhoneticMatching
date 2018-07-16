/**
 * @file
 * Levenshtein (min edit) distance.
 *
 * @author Benedicte Pierrejean
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_LEVENSHTEIN_HPP
#define MALUUBA_LEVENSHTEIN_HPP

#include "maluuba/metric.hpp"
#include <algorithm>
#include <iterator>
#include <memory>
#include <type_traits>
#include <utility>

namespace maluuba
{
  /**
   * Cost functor that always returns the same value.
   *
   * @tparam T
   *         The numeric type to return.
   *
   * @author Tavian Barnes (tavian.barnes@microsoft.com)
   */
  template <typename T>
  class ConstantCost
  {
  public:
    ConstantCost(T cost = T{1})
      : m_cost{cost}
    { }

    template <typename U>
    T
    operator()(const U& u) const
    {
      return m_cost;
    }

  private:
    T m_cost;
  };

  /**
   * Levenshtein distance metric.
   *
   * @tparam SubstitutionMetric
   *         The metric used to compute substitution costs.
   * @tparam CostFunction
   *         The cost function for insertions/deletions.
   *
   * @author Benedicte Pierrejean
   * @author Tavian Barnes (tavian.barnes@microsoft.com)
   */
  template <typename SubstitutionMetric = EqualityMetric,
            typename CostFunction = ConstantCost<MetricResult<SubstitutionMetric>>>
  class LevenshteinDistance
  {
    template <typename T>
    using ValueType = typename std::iterator_traits<typename T::iterator>::value_type;

    template <typename T, typename U>
    using ResultType = MetricResult<SubstitutionMetric, ValueType<T>, ValueType<U>>;

  public:
    /**
     * Create a @c LevenshteinDistance.
     *
     * @param sub_metric  The metric used to compute substitution costs.
     * @param cost  The cost (function) for insertions/deletions (default: 1).
     */
    LevenshteinDistance(SubstitutionMetric sub_metric = SubstitutionMetric{},
                        CostFunction cost = CostFunction{})
      : m_sub_metric(std::move(sub_metric)),
        m_cost(std::move(cost))
    { }

    template <typename T, typename U>
    ResultType<T, U>
    operator()(const T& t_seq, const U& u_seq) const
    {
      // Wagner-Fischer algorithm with two active rows

      using Number = ResultType<T, U>;

      auto cols = u_seq.size() + 1;
      auto row0 = std::make_unique<Number[]>(cols);
      auto row1 = std::make_unique<Number[]>(cols);

      Number initial_cost{};
      std::size_t i = 0;
      row0[i] = initial_cost;
      for (const auto& u : u_seq) {
        ++i;
        initial_cost += m_cost(u);
        row0[i] = initial_cost;
      }

      for (const auto& t : t_seq) {
        auto t_cost = m_cost(t);
        row1[0] = row0[0] + t_cost;

        i = 1;
        for (const auto& u : u_seq) {
          auto sub_cost = row0[i - 1] + m_sub_metric(t, u);
          auto del_cost = row0[i] + t_cost;
          auto ins_cost = row1[i - 1] + m_cost(u);
          row1[i] = std::min(sub_cost, std::min(del_cost, ins_cost));
          ++i;
        }

        std::swap(row0, row1);
      }

      return row0[cols - 1];
    }

  private:
    SubstitutionMetric m_sub_metric;
    CostFunction m_cost;
  };
}

#endif // MALUUBA_LEVENSHTEIN_HPP
