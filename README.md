[![Build Status](https://travis-ci.org/Microsoft/PhoneticMatching.svg?branch=master)](https://travis-ci.org/Microsoft/PhoneticMatching)
[![Build Status](https://ci.appveyor.com/api/projects/status/github/Microsoft/PhoneticMatching?branch=master&svg=true)](https://ci.appveyor.com/projects)

# Introduction
A phonetic matching library. Includes text utilities to do string comparisons on phonemes (the sound of the string), as opposed to characters.

Docs can be found at: https://microsoft.github.io/PhoneticMatching/

Supported API:
* C++
* Node.js (>=8.11.2)

Supported Languages
* English

Current pre-built binaries offered to save the trouble of compiling the source locally.
* node-v{64,59,57}-{win32}-{x64,x86}
* node-v{64,59,57}-{linux}-{x64}

(Run `node -p "process.versions.modules"` to see which Node-ABI in use.)
# Getting Started
This repository consists of TypeScript and native dependencies built with `node-gyp`. See `package.json` for various scripts for the developement process.

For first time building remember to `npm install`

## Install
To install from NPM
```
npm install phoneticmatching
```

## Usage
See the typings for more details. <br> Classes prefixed with `En` make certain assumptions that are specific to the English language.
```ts
import maluuba, { EnPronouncer, EnPhoneticDistance, FuzzyMatcher, AcceleratedFuzzyMatcher, EnHybridDistance, StringDistance } from "phoneticmatching";
```
__maluuba__ Default export, contains everything below.

__Speech__ The namespace containing the type interfaces of the library objects.

__FuzzyMatcher__ Main use case for this library. Returns matches against a list of targets for a given query. The comparisions are not remembered and therefore better for one-off use cases.

__AcceleratedFuzzyMatcher__ Same interface as `FuzzyMatcher` but the list of targets are precomputed, so beneficial for multiple queries at the cost of a higher initialization time.

__EnPronouncer__ Pronounces a string, as a General English speaker, into its IPA string or array of Phones format.

__EnPhoneticDistance__ Returns a metric distance score between two English pronunciations.

__StringDistance__ Returns a metric distance score between two strings (edit distance).

__EnHybridDistance__ Returns a metric distance score based on a combination of the two above distance metrics (English pronunciations and strings).

```ts
import { EnContactMatcher, EnPlaceMatcher } from "phoneticmatching/lib/matchers";
```
__EnContactMatcher__ A domain specialization of using the `AcceleratedFuzzyMatcher` for English speakers searching over a list of names. Does additional preprocessing and setups up the distance function for you.

__EnPlaceMatcher__ A domain specialization of using the `AcceleratedFuzzyMatcher` for English speakers searching over a list of places. Does additional preprocessing and setups up the distance function for you.

## Example
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
# Requires native depedencies built, but TypeScript transpiling not required.
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
Note that the .js library code and native dependencies will be deployed separately. Npm regsitries will be used for the .js code, `node-pre-gyp` will be used for prebuilt dependencies while falling back to building on the client.
```py
# Pushes pack to npmjs.com or a private registry if a .npmrc exisits.
npm publish
```
```py
# Packages a ./build/stage/{version}/maluubaspeech-{node_abi}-{platform}-{arch}.tar.gz.
# See package.json:binary.host on where to put it.
npm run package
```

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
