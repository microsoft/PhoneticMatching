/**
 * @file
 * Macro to export symbols.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

#include "maluuba/levenshtein.hpp"
#include "maluuba/speech/csharp/csharp.hpp"
#include "maluuba/speech/fuzzymatcher.hpp"
#include "maluuba/speech/hybriddistance.hpp"
#include "maluuba/speech/phoneticdistance.hpp"
#include "maluuba/speech/pronouncer.hpp"
#include "maluuba/speech/pronunciation.hpp"
#include "maluuba/unicode.hpp"

#include <string>
#include <stdexcept>
#include <vector>

using namespace maluuba::speech;

enum class Result
{
    SUCCESS,
    INVALID_PARAMETER,
    INTERNAL_ERROR,
    BUFFER_TOO_SMALL
};

template <typename CHAR_T>
bool 
copy_to_buffer(CHAR_T* buffer, size_t& bufferSize, const CHAR_T* msg)
{
    const unsigned int stringLength = std::char_traits<CHAR_T>::length(msg);

    if (bufferSize >= (stringLength + 1)) {
        while((*buffer++ = *msg++) != 0);

        // current buffer is enough
        return true;
    } else {
        // Return the number of characters needed (including null termination)
        bufferSize = stringLength + 1;
        return false;
    }
}

Result 
HandleException(char* buffer, size_t* bufferSize) noexcept
{
    try {
        throw;
    } catch (const std::domain_error& ex) {
        if (!copy_to_buffer(buffer, *bufferSize, ex.what())) {
            return Result::BUFFER_TOO_SMALL;
        }
        return Result::INVALID_PARAMETER;
    } catch (const std::invalid_argument& ex) {
        if (!copy_to_buffer(buffer, *bufferSize, ex.what())) {
            return Result::BUFFER_TOO_SMALL;
        }
        return Result::INVALID_PARAMETER;
    } catch (const std::exception& ex) {
        if (!copy_to_buffer(buffer, *bufferSize, ex.what())) {
            return Result::BUFFER_TOO_SMALL;
        }
        return Result::INTERNAL_ERROR;
    }
}

template <typename T>
Result 
NativeDelete(T* ptr, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
{
    try {
        if (ptr != NULL) {
            delete ptr;
            ptr = NULL;
        }

        return Result::SUCCESS;
    } catch (const std::exception&) {
        return HandleException(buffer, bufferSize);
    }
}

void 
CheckPointer(void* ptr)
{
    if (!ptr) {
        throw std::invalid_argument("pointer is null");
    }
}

extern "C" 
{
    /*
     * StringDistance
     */

    DLL_PUBLIC 
    Result 
    StringDistance_Create(/*out*/ maluuba::LevenshteinDistance<>** native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize) 
    {
        try {
            *native = new maluuba::LevenshteinDistance<>();
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    StringDistance_Distance(maluuba::LevenshteinDistance<>* ptr, const char* a, const char* b, /*out*/ double* distance, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            CheckPointer(ptr);
            *distance = (*ptr)(std::string(a), std::string(b));
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    StringDistance_Delete(maluuba::LevenshteinDistance<>* native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        return NativeDelete(native, buffer, bufferSize);
    }

    /*
     * EnPhoneticDistance
     */

    DLL_PUBLIC 
    Result 
    EnPhoneticDistance_Create(/*out*/EnPhoneticDistance** native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            *native = new EnPhoneticDistance();
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    EnPhoneticDistance_Distance(EnPhoneticDistance* ptr, const EnPronunciation* a, const EnPronunciation* b, /*out*/ double* distance, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            CheckPointer(ptr);
            *distance = (*ptr)(*a, *b);
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    EnPhoneticDistance_Delete(EnPhoneticDistance* native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        return NativeDelete(native, buffer, bufferSize);
    }

    /*
     * EnHybridDistance
     */

    DLL_PUBLIC 
    Result 
    EnHybridDistance_Create(double phonetic_weight_percentage, /*out*/ HybridDistance<>** native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            *native = new HybridDistance<>(phonetic_weight_percentage);
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    EnHybridDistance_Distance(HybridDistance<>* ptr, const char* a_phrase, EnPronunciation* a_pronunciation, const char* b_phrase, EnPronunciation* b_pronunciation, /*out*/ double* distance, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            CheckPointer(ptr);
            std::string a_string(a_phrase);
            std::string b_string(b_phrase);
            *distance = (*ptr)(a_string, *a_pronunciation, b_string, *b_pronunciation);
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    EnHybridDistance_Delete(HybridDistance<>* native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        return NativeDelete(native, buffer, bufferSize);
    }

    /*
     * EnPronouncer
     */

    DLL_PUBLIC 
    Result 
    EnPronouncer_Create(/*out*/ EnPronouncer** native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            *native = new EnPronouncer();
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    EnPronouncer_Pronounce(EnPronouncer* ptr, const char* phrase, /*out*/EnPronunciation** native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            CheckPointer(ptr);
            *native = new EnPronunciation(ptr->pronounce(phrase));
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    EnPronouncer_Delete(EnPronouncer* native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        return NativeDelete(native, buffer, bufferSize);
    }

    /*
     * EnPronunciation
     */

    DLL_PUBLIC 
    Result 
    EnPronunciation_FromArpabet(const char** head, const int count, /*out*/ EnPronunciation** ret, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            *ret = new EnPronunciation(EnPronunciation::from_arpabet(std::vector<std::string>(head, head + count)));
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    EnPronunciation_FromIpa(const char* ipa, /*out*/EnPronunciation** native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            EnPronunciation pronunciation = EnPronunciation::from_ipa(ipa);
            *native = new EnPronunciation(pronunciation);
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    EnPronunciation_Delete(EnPronunciation* native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        return NativeDelete(native, buffer, bufferSize);
    }

    DLL_PUBLIC 
    Result 
    EnPronunciation_Ipa(EnPronunciation* ptr, /*out*/ char16_t* ipa, /*out*/ char* errorBuffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            CheckPointer(ptr);
            std::string str = ptr->to_ipa();
            std::u16string wstr = maluuba::unicode_cast<std::u16string>(str.c_str());
            
            if (!copy_to_buffer(ipa, *bufferSize, wstr.c_str())) {
                return Result::BUFFER_TOO_SMALL;
            }

            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(errorBuffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    EnPronunciation_Count(EnPronunciation* ptr, /*out*/int* count, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            CheckPointer(ptr);
            *count = ptr->size();
            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    struct PhoneFields
    {
        PhoneType type;
        Phonation phonation;
        PlaceOfArticulation place;
        MannerOfArticulation manner;
        VowelHeight height;
        VowelBackness backness;
        VowelRoundedness roundedness;
        int isRhotic;
        int isSyllabic;
    };

    DLL_PUBLIC 
    Result 
    EnPronunciation_Phones(EnPronunciation* ptr, /*in,out*/ PhoneFields* fields, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            CheckPointer(ptr);
            Pronunciation::iterator it = ptr->begin();

            int idx = 0;
            while (it != ptr->end()) {
                auto type = it->type();
                fields[idx].type = type;
                fields[idx].phonation = it->phonation();
                fields[idx].isSyllabic = it->is_syllabic();
                
                if (type == PhoneType::VOWEL)
                {
                    fields[idx].height = it->height();
                    fields[idx].backness = it->backness();
                    fields[idx].roundedness = it->roundedness();
                    fields[idx].isRhotic = it->is_rhotic();
                }
                else
                {
                    fields[idx].place = it->place();
                    fields[idx].manner = it->manner();
                }

                ++idx;
                ++it;
            }

            return Result::SUCCESS;
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    /*
     * FuzzyMatcher
     */

    /* Declare an unmanaged function type that takes two int arguments  
       Note the use of __stdcall for compatibility with managed code */  
    typedef double (STDCALL *CALLBACK)(int, int); 

    DLL_PUBLIC 
    Result 
    FuzzyMatcher_Create(const int count, CALLBACK distance, const bool isAccelerated, /*out*/FuzzyMatcher<int>** ret, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        Result result;
        int* targets = new int[count];
        try {
            for (int idx = 0; idx < count; ++idx) {
                targets[idx] = idx;
            }

            if (isAccelerated) {
                *ret = new AcceleratedFuzzyMatcher<int, CALLBACK>(targets, targets + count, distance);
            } else {
                *ret = new LinearFuzzyMatcher<int, CALLBACK>(targets, targets + count, distance);
            }


            result = Result::SUCCESS;
        } catch (const std::exception&) {
            result = HandleException(buffer, bufferSize);
        }
        
        delete[] targets;
        return result;
    }

    DLL_PUBLIC 
    Result 
    FuzzyMatcher_Delete(FuzzyMatcher<int>* native, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        return NativeDelete(native, buffer, bufferSize);
    }

    Result 
    ProcessNearestElements(const std::vector<FuzzyMatcher<int>::Match>& matches, int* nearestIdxs, double* distances)
    {
        for (size_t idx = 0; idx < matches.size(); ++idx) {
            nearestIdxs[idx] = matches[idx].element();
            distances[idx] = matches[idx].distance();
        }

        return Result::SUCCESS;
    }

    DLL_PUBLIC 
    Result 
    FuzzyMatcher_FindNearestWithin(LinearFuzzyMatcher<int, CALLBACK>* ptr, int capacity, double limit, int* nearestIdxs, double* distances, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            CheckPointer(ptr);
            return ProcessNearestElements(ptr->find_k_nearest_within(-1, capacity, limit), nearestIdxs, distances);
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }

    DLL_PUBLIC 
    Result 
    AcceleratedFuzzyMatcher_FindNearestWithin(AcceleratedFuzzyMatcher<int, CALLBACK>* ptr, int capacity, double limit, int* nearestIdxs, double* distances, /*out*/ char* buffer, /*in,out*/ size_t* bufferSize)
    {
        try {
            CheckPointer(ptr);
            return ProcessNearestElements(ptr->find_k_nearest_within(-1, capacity, limit), nearestIdxs, distances);
        } catch (const std::exception&) {
            return HandleException(buffer, bufferSize);
        }
    }
}