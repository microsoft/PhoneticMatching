{
    "target_defaults": {
        "include_dirs": [
            "src",
        ],
        "cflags_cc": [
            "-std=c++17",
            "-Wall",
            "-pedantic",
        ],
        "cflags_cc!": [
            "-std=gnu++0x",
            "-std=gnu++1y",
            "-fno-exceptions",
            "-fno-rtti",
        ],
        "cflags!": [
            "-Wall",
            "-Wextra",
        ],
        # "defines!":["DEBUG"], # Turn off really loud dependencies
        "msvs_settings": {
            "VCCLCompilerTool": {
                "AdditionalOptions": [
                    "/std:c++17",
                    "/permissive-",
                    "/Zc:__cplusplus",
                    "/utf-8",
                    "/WX-",
                ],
            },
        },
        # To disable default flags for macOS, see each target, "target_defaults" didn't seem to override.
        # To disable default flags, windows has to override starting from the configurations.
        "configurations": {
            "Release": {
                "msvs_settings": {
                    "VCCLCompilerTool": {
                        "AdditionalOptions": [
                            "/EHa", # /EHs[c] has deleted objects?
                        ],
                        "WarningLevel": 1,
                    }
                }
            },
            "Debug": {
                "msvs_settings": {
                    "VCCLCompilerTool": {
                        "AdditionalOptions": [
                            "/wd4530", # Debug with /EH seems to abort?
                        ],
                        "WarningLevel": 1,
                    }
                }
            }
        },
    },
    "targets": [
        {
            "target_name": "maluubaspeech-csharp",
            "dependencies": [
                "maluubaspeech-source",
            ],
            "sources": [
                "src/maluuba/speech/csharp/bindings.cpp"
            ],
            "xcode_settings": {
                "CLANG_CXX_LANGUAGE_STANDARD": "c++17",
                "GCC_ENABLE_CPP_EXCEPTIONS": "YES", # remove -fno-exceptions
                "GCC_ENABLE_CPP_RTTI": "YES", # remove -fno-rtti
                "OTHER_CFLAGS+": [
                    "-Wall",
                    "-pedantic",
                ],
                "WARNING_CFLAGS!": [
                    "-Wall",
                    "-Wextra",
                ],
            },
        },
        {
            "target_name": "maluubaspeech",
            "dependencies": [
                "maluubaspeech-source",
            ],
            "sources": [
                "src/maluuba/speech/nodejs/enhybriddistance/enhybriddistance.cpp",
                "src/maluuba/speech/nodejs/enphoneticdistance/enphoneticdistance.cpp",
                "src/maluuba/speech/nodejs/enpronouncer/enpronouncer.cpp",
                "src/maluuba/speech/nodejs/enpronunciation/enpronunciation.cpp",
                "src/maluuba/speech/nodejs/main.cpp",
                "src/maluuba/speech/nodejs/match/match.cpp",
                "src/maluuba/speech/nodejs/performance/performance.cpp",
                "src/maluuba/speech/nodejs/phone/phone.cpp",
                "src/maluuba/speech/nodejs/stringdistance/stringdistance.cpp",
            ],
            "xcode_settings": {
                "CLANG_CXX_LANGUAGE_STANDARD": "c++17", # -std=c++17
                "GCC_ENABLE_CPP_EXCEPTIONS": "YES", # remove -fno-exceptions
                "GCC_ENABLE_CPP_RTTI": "YES", # remove -fno-rtti
                "OTHER_CFLAGS+": [
                    "-Wall",
                    "-pedantic",
                ],
            },
        },
        {
            "target_name": "maluubaspeech-source",
            "type": "static_library",
            "defines": [
                "MALUUBA_CODECVT_BUG=_MSC_VER >= 1900"
            ],
            "dependencies": [
                "flite",
            ],
            "sources": [
                "src/maluuba/speech/phoneticdistance/metric.cpp",
                "src/maluuba/speech/phoneticdistance/phoneticdistance.cpp",
                "src/maluuba/speech/pronouncer/pronouncer.cpp",
                "src/maluuba/speech/pronunciation/arpabet.cpp",
                "src/maluuba/speech/pronunciation/ipa.cpp",
                "src/maluuba/speech/pronunciation/phone.cpp",
                "src/maluuba/speech/pronunciation/pronunciation.cpp",
                "src/maluuba/unicode/unicode.cpp",
            ],
            "xcode_settings": {
                "CLANG_CXX_LANGUAGE_STANDARD": "c++17", # -std=c++17
                "GCC_ENABLE_CPP_EXCEPTIONS": "YES", # remove -fno-exceptions
                "GCC_ENABLE_CPP_RTTI": "YES", # remove -fno-rtti
                "OTHER_CFLAGS+": [
                    "-Wall",
                    "-pedantic",
                ],
            },
        },
        {
            "target_name": "flite",
            "type": "static_library",
            "direct_dependent_settings": {
                "include_dirs": [
                    "src/flite/include",
                ],
            },
            "include_dirs": [
                "src/flite/include",
            ],
            "cflags": [
                "-Wno-int-to-pointer-cast",
                "-Wno-pointer-to-int-cast",
            ],
            # overriding defaults
            "configurations": {
                "Release": {
                    "msvs_settings": {
                        "VCCLCompilerTool": {
                            "WarningLevel": 0,
                        }
                    }
                },
                "Debug": {
                    "msvs_settings": {
                        "VCCLCompilerTool": {
                            "WarningLevel": 0,
                        }
                    }
                }
            },
            "sources": [
                # flite_cmulex
                "src/flite/lang/cmulex/cmu_lex_data.c",
                "src/flite/lang/cmulex/cmu_lex_entries.c",
                "src/flite/lang/cmulex/cmu_lex.c",
                "src/flite/lang/cmulex/cmu_lts_model.c",
                "src/flite/lang/cmulex/cmu_lts_rules.c",
                "src/flite/lang/cmulex/cmu_postlex.c",
                # flite_usenglish
                "src/flite/lang/usenglish/us_aswd.c",
                "src/flite/lang/usenglish/us_dur_stats.c",
                "src/flite/lang/usenglish/us_durz_cart.c",
                "src/flite/lang/usenglish/usenglish.c",
                "src/flite/lang/usenglish/us_expand.c",
                "src/flite/lang/usenglish/us_f0lr.c",
                "src/flite/lang/usenglish/us_f0_model.c",
                "src/flite/lang/usenglish/us_ffeatures.c",
                "src/flite/lang/usenglish/us_gpos.c",
                "src/flite/lang/usenglish/us_int_accent_cart.c",
                "src/flite/lang/usenglish/us_int_tone_cart.c",
                "src/flite/lang/usenglish/us_nums_cart.c",
                "src/flite/lang/usenglish/us_phoneset.c",
                "src/flite/lang/usenglish/us_phrasing_cart.c",
                "src/flite/lang/usenglish/us_pos_cart.c",
                "src/flite/lang/usenglish/us_text.c",
                # flite
                "src/flite/src/audio/audio.c",
                "src/flite/src/audio/au_none.c",
                "src/flite/src/audio/auserver.c",
                "src/flite/src/audio/au_streaming.c",
                "src/flite/src/cg/cst_cg.c",
                "src/flite/src/cg/cst_cg_dump_voice.c",
                "src/flite/src/cg/cst_cg_load_voice.c",
                "src/flite/src/cg/cst_cg_map.c",
                "src/flite/src/cg/cst_mlpg.c",
                "src/flite/src/cg/cst_mlsa.c",
                "src/flite/src/cg/cst_spamf0.c",
                "src/flite/src/cg/cst_vc.c",
                "src/flite/src/hrg/cst_ffeature.c",
                "src/flite/src/hrg/cst_item.c",
                "src/flite/src/hrg/cst_relation.c",
                "src/flite/src/hrg/cst_rel_io.c",
                "src/flite/src/hrg/cst_utterance.c",
                "src/flite/src/lexicon/cst_lexicon.c",
                "src/flite/src/lexicon/cst_lts.c",
                "src/flite/src/lexicon/cst_lts_rewrites.c",
                "src/flite/src/regex/cst_regex.c",
                "src/flite/src/regex/regexp.c",
                "src/flite/src/regex/regsub.c",
                "src/flite/src/speech/cst_lpcres.c",
                "src/flite/src/speech/cst_track_io.c",
                "src/flite/src/speech/cst_track.c",
                "src/flite/src/speech/cst_wave_io.c",
                "src/flite/src/speech/cst_wave.c",
                "src/flite/src/speech/cst_wave_utils.c",
                "src/flite/src/speech/g721.c",
                "src/flite/src/speech/g723_24.c",
                "src/flite/src/speech/g723_40.c",
                "src/flite/src/speech/g72x.c",
                "src/flite/src/speech/rateconv.c",
                "src/flite/src/stats/cst_cart.c",
                "src/flite/src/stats/cst_ss.c",
                "src/flite/src/stats/cst_viterbi.c",
                "src/flite/src/synth/cst_ffeatures.c",
                "src/flite/src/synth/cst_phoneset.c",
                "src/flite/src/synth/cst_ssml.c",
                "src/flite/src/synth/cst_synth.c",
                "src/flite/src/synth/cst_utt_utils.c",
                "src/flite/src/synth/cst_voice.c",
                "src/flite/src/synth/flite.c",
                "src/flite/src/utils/cst_alloc.c",
                "src/flite/src/utils/cst_args.c",
                "src/flite/src/utils/cst_endian.c",
                "src/flite/src/utils/cst_error.c",
                "src/flite/src/utils/cst_features.c",
                "src/flite/src/utils/cst_file_stdio.c",
                "src/flite/src/utils/cst_socket.c",
                "src/flite/src/utils/cst_string.c",
                "src/flite/src/utils/cst_tokenstream.c",
                "src/flite/src/utils/cst_url.c",
                "src/flite/src/utils/cst_val_const.c",
                "src/flite/src/utils/cst_val.c",
                "src/flite/src/utils/cst_val_user.c",
                "src/flite/src/utils/cst_wchar.c",
                "src/flite/src/wavesynth/cst_clunits.c",
                "src/flite/src/wavesynth/cst_diphone.c",
                "src/flite/src/wavesynth/cst_reflpc.c",
                "src/flite/src/wavesynth/cst_sigpr.c",
                "src/flite/src/wavesynth/cst_sts.c",
                "src/flite/src/wavesynth/cst_units.c",
            ],
            "xcode_settings": {
                "WARNING_CFLAGS!": [
                    "-Wall",
                    "-Wextra",
                ],
                "OTHER_CFLAGS+": [
                    "-Wno-sign-compare",
                    "-Wno-absolute-value",
                ],
            },
            "conditions": [
                ["OS=='win'", {
                    "defines": [
                        "CST_NO_SOCKETS",
                    ],
                    "sources": [
                        "src/flite/src/utils/cst_mmap_win32.c",
                    ],
                }, { # "OS!='win'"
                    "sources": [
                        "src/flite/src/utils/cst_mmap_posix.c",
                    ],
                }],
            ]
        },
        {
            "target_name": "action_after_build",
            "type": "none",
            "dependencies": [
                "<(module_name)"
            ],
            "copies": [
                {
                    "files": [
                        "<(PRODUCT_DIR)/<(module_name).node"
                    ],
                    "destination": "<(module_path)"
                }
            ]
        }
    ]
}
