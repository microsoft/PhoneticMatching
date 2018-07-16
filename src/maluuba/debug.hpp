/**
 * @file
 * Debuging utilities.
 *
 * @author Tavian Barnes (tavian.barnes@microsoft.com)
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_DEBUG_HPP
#define MALUUBA_DEBUG_HPP

#include <exception>
#include <stdexcept>
#include <string>
#include <utility>

namespace maluuba
{
  /**
   * Check that a condition is true, and throw an exception if not.
   *
   * @tparam E  The type of exception to throw.
   * @param condition  The condition that must hold.
   * @param args  Arguments to pass to the exception constructor.
   * @throws E  If <code>!condition</code>.
   */
  template <typename E = std::runtime_error, typename Cond, typename... Args>
  void
  check(Cond&& condition, Args&&... args)
  {
    if (!static_cast<bool>(condition)) {
      throw E(std::forward<Args>(args)...);
    }
  }

  /**
   * Check that a condition is true, and throw @c std::logic_error if not.
   *
   * @param condition  The condition that must hold.
   * @param message  The exception message for failures.
   * @throws std::logic_error  If <code>!condition</code>.
   */
  template <typename Cond>
  void
  check_logic(Cond&& condition, const char* message)
  {
    check<std::logic_error>(std::forward<Cond>(condition), message);
  }
}

#endif // MALUUBA_DEBUG_HPP
