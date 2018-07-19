// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import Soundex from "./soundex";

test("Soundex.", () => {
    expect(Soundex.encode("")).toBe("");
    expect(Soundex.encode(" ")).toBe("");

    expect(Soundex.encode("Robert")).toBe("R163");
    expect(Soundex.encode("Rupert")).toBe("R163");
    expect(Soundex.encode("Rubin")).toBe("R150");
    expect(Soundex.encode("Ashcraft")).toBe("A261");
    expect(Soundex.encode("Ashcroft")).toBe("A261");
    expect(Soundex.encode("Tymczak")).toBe("T522");
    expect(Soundex.encode("Pfister")).toBe("P236");
    expect(Soundex.encode("Honeyman")).toBe("H555");

    expect(Soundex.encode("Robert Robert")).toBe("R163 R163");
});
