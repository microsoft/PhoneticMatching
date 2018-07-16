/**
 * @file
 * Distance metrics.
 *
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_METRIC_HPP
#define MALUUBA_METRIC_HPP

#include <type_traits>
#include <utility>

namespace maluuba
{
  /**
   * Infers the result type of a distance metric.
   */
  template <typename Metric, typename T = int, typename U = T>
  using MetricResult = std::result_of_t<Metric(T, U)>;

  /**
   * Equality distance metric.
   *
   * @author Tavian Barnes (tavian.barnes@microsoft.com)
   */
  class EqualityMetric
  {
  public:
    /**
     * Compute the distance between @p t and @p u.
     *
     * @return 0 if <code>t == u</code>, 1 otherwise.
     */
    template <typename T, typename U>
    int
    operator()(const T& t, const U& u) const
    {
      return t == u ? 0 : 1;
    }
  };
}

#endif // MALUUBA_METRIC_HPP
