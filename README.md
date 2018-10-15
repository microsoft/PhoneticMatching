[![Build Status](https://travis-ci.org/Microsoft/PhoneticMatching.svg?branch=master)](https://travis-ci.org/Microsoft/PhoneticMatching)
[![Build Status](https://ci.appveyor.com/api/projects/status/github/Microsoft/PhoneticMatching?branch=master&svg=true)](https://ci.appveyor.com/projects)

# Introduction
A phonetic matching library. Includes text utilities to do string comparisons on phonemes (the sound of the string), as opposed to characters.

Docs can be found at: https://microsoft.github.io/PhoneticMatching/

Supported API:
* C++	
* Node.js (>=8.11.2)
* C# .NET Core (>=2.1)

Supported Languages
* English

Current pre-built binaries offered to save the trouble of compiling the source locally.
* node-v{64,59,57}-{win32}-{x64,x86}
* node-v{64,59,57}-{linux,darwin}-{x64}

(Run `node -p "process.versions.modules"` to see which Node-ABI in use.)
# Getting Started
This repository consists of TypeScript and native dependencies built with `node-gyp`. See `package.json` for various scripts for the development process.

For first time building remember to `npm install`

This repository uses git submodules. If paths are outdated or non-existent run `git submodule update --init --recursive`

## Install
To install from NPM
```
npm install phoneticmatching
```

## Usage
See the typings for more details. <br> Classes prefixed with `En` make certain assumptions that are specific to the English language.
```ts
import { EnPronouncer, EnPhoneticDistance, FuzzyMatcher, AcceleratedFuzzyMatcher, EnHybridDistance, StringDistance } from "phoneticmatching";
```
__Speech__ The namespace containing the type interfaces of the library objects.

__EnPronouncer__ Pronounces a string, as a General English speaker, into its IPA string or array of Phones format.

__matchers__ module:

* __FuzzyMatcher__ Main use case for this library. Returns matches against a list of targets for a given query. The comparisions are not remembered and therefore better for one-off use cases.

* __AcceleratedFuzzyMatcher__ Same interface as `FuzzyMatcher` but the list of targets are precomputed, so beneficial for multiple queries at the cost of a higher initialization time.

* __EnContactMatcher__ A domain specialization of using the `AcceleratedFuzzyMatcher` for English speakers searching over a list of names. Does additional preprocessing and setups up the distance function for you.

* __EnPlaceMatcher__ A domain specialization of using the `AcceleratedFuzzyMatcher` for English speakers searching over a list of places. Does additional preprocessing and setups up the distance function for you.

__distance__ module:

* __EnPhoneticDistance__ Returns a metric distance score between two English pronunciations.

* __StringDistance__ Returns a metric distance score between two strings (edit distance).

* __EnHybridDistance__ Returns a metric distance score based on a combination of the two above distance metrics (English pronunciations and strings).

* __DistanceInput__ Input object for EnHybridDistance. Hold the text and the pronunciation of that text

__nlp__ module:

* __EnPreProcessor__ English Pre-processor.

* __EnPlacesPreProcessor__ English Pre-processor with specific rules for places.

* __SplittingTokenizer__ Tokenizing base-class that will split on the given RegExp.

Here are some example of how to import modules and classes:

```ts
import { EnContactMatcher, EnPlaceMatcher } from "phoneticmatching";
```
```ts
import * as Matchers from "phoneticmatching/lib/matchers";
```

## Example
JavaScript
```js
// Import core functionality from the library.
const { EnPhoneticDistance, FuzzyMatcher } = require("phoneticmatching");

// A distance metric over pronunciations.
const metric = new EnPhoneticDistance();

// The target list to match against.
const targets = [
    "Apple",
    "Banana",
    "Blackberry",
    "Blueberry",
    "Grapefruit",
    "Pineapple",
    "Raspberry",
    "Strawberry",
];

// Create the fuzzy matcher.
const matcher = new FuzzyMatcher(targets, metric);
// Find the nearest match.
const result = matcher.nearest("blu airy");
/* The result should be:
 * {
 *     // The object from the targets list.
 *     element: 'Blueberry',
 *     // The distance score the from distance function.
 *     distance: 0.041666666666666664
 * }
 */
console.log(result);
```
C#
```csharp
using System;

// Import core functionality from the library.
using Microsoft.PhoneticMatching.Matchers.FuzzyMatcher.Normalized;

public class Program
{
    public static void Main(string[] args)
    {
        // The target list to match against.
        string[] targets = 
        {
            "Apple",
            "Banana",
            "Blackberry",
            "Blueberry",
            "Grapefruit",
            "Pineapple",
            "Raspberry",
            "Strawberry",
        };

        // Create the fuzzy matcher.
        var matcher = new EnPhoneticFuzzyMatcher<string>(targets);

        // Find the nearest match.
        var result = matcher.FindNearest("blu airy");

        /* The result should be:
         * {
         *     // The object from the targets list.
         *     element: 'Blueberry',
         *     // The distance score the from distance function.
         *     distance: 0.0416666666666667
         * }
         */
        Console.WriteLine("element : [{0}] - distance : [{1}]", result.Element, result.Distance);
    }
}
```

## Build
### TypeScript Transpiling
```
npm run tsc
```
### Native Compiling
```py
# X is the parallelization number, usually set to the number of cores of the machine.
# This cleans and rebuilds everything.
JOBS=X npm run rebuild
# For incremental builds.
JOBS=X npm run build
```

## Test
```py
# Requires native dependencies built, but TypeScript transpiling not required.
npm test
```

## Docs
```py
# Generate the doc files from the docstrings.
npm run build-docs
```

## Release
```py
# Builds everything, TypeScript & native & docs, as a release build.
npm run release
```

## Deployment/Upload
Note that the .js library code and native dependencies will be deployed separately. Npm registries will be used for the .js code, `node-pre-gyp` will be used for prebuilt dependencies while falling back to building on the client.
```py
# Pushes pack to npmjs.com or a private registry if a .npmrc exists.
npm publish
```
```py
# Packages a ./build/stage/{version}/maluubaspeech-{node_abi}-{platform}-{arch}.tar.gz.
# See package.json:binary.host on where to put it.
npm run package
```

## NuGet Publish
A .NET Core NuGet package is published for this project. The package is published by Microsoft. Hence, it must follow guidance at https://aka.ms/nuget and sign package content and package itself with an official Microsoft certificate. To ease signing and publishing process, we integrate ESRP signing to Azure DevOps build tasks.
To publish a new version of the package, create a release for the latest build (Pipelines->Releases->PublishNuget->Create a release).

# Contributors
This project welcomes contributions and suggestions. Most contributions require you to
agree to a Contributor License Agreement (CLA) declaring that you have the right to,
and actually do, grant us the rights to use your contribution. For details, visit
https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need
to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the
instructions provided by the bot. You will only need to do this once across all repositories using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Reporting Security Issues

Security issues and bugs should be reported privately, via email, to the Microsoft Security
Response Center (MSRC) at [secure@microsoft.com](mailto:secure@microsoft.com). You should
receive a response within 24 hours. If for some reason you do not, please follow up via
email to ensure we received your original message. Further information, including the
[MSRC PGP](https://technet.microsoft.com/en-us/security/dn606155) key, can be found in
the [Security TechCenter](https://technet.microsoft.com/en-us/security/default).

# License
Copyright (c) Microsoft Corporation. All rights reserved.

Licensed under the MIT License.

See sources for licenses of dependencies.
