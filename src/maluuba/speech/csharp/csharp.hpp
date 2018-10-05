/**
 * @file
 * Macro to export symbols.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#ifndef MALUUBA_SPEECH_CSHARP_CSHARP_HPP
#define MALUUBA_SPEECH_CSHARP_CSHARP_HPP


#if defined _WIN32 || defined __CYGWIN__
  #define STDCALL __stdcall
  #if 1 //def BUILDING_DLL
    #ifdef __GNUC__
      #define DLL_PUBLIC __attribute__ ((dllexport))
    #else
      #define DLL_PUBLIC __declspec(dllexport) // Note: actually gcc seems to also supports this syntax.
    #endif
  #else
    #ifdef __GNUC__
      #define DLL_PUBLIC __attribute__ ((dllimport))
    #else
      #define DLL_PUBLIC __declspec(dllimport) // Note: actually gcc seems to also supports this syntax.
    #endif
  #endif    // BUILDING_DLL
  #define DLL_LOCAL
#else
  #define STDCALL
  #if __GNUC__ >= 4
    #define DLL_PUBLIC __attribute__ ((visibility ("default")))
    #define DLL_LOCAL  __attribute__ ((visibility ("hidden")))
  #else
    #define DLL_PUBLIC
    #define DLL_LOCAL
  #endif    // __GNUC__ >= 4
#endif  // defined _WIN32 || defined __CYGWIN__

#endif  // MALUUBA_SPEECH_CSHARP_CSHARP_HPP