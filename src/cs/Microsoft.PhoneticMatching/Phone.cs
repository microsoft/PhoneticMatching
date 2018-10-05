// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching
{
    /// <summary>
    /// Phone type (consonant or vowel)
    /// </summary>
    public enum PhoneType
    {
        Consonant,
        Vowel
    }

    /// <summary>
    /// Phonation (voice intensity)
    /// </summary>
    public enum Phonation
    {
        Voiceless,
        Breathy,
        Slack,
        Modal,
        Stiff,
        Creaky,
        GlottalClosure,
    }

    /// <summary>
    /// Place of articulation for consonants.
    /// </summary>
    public enum PlaceOfArticulation
    {
        Bilabial,
        Labiodental,
        Dental,
        Alveolar,
        PalatoAlveolar,
        Retroflex,
        AlveoloPalatal,
        LabialPalatal,
        Palatal,
        PalatalVelar,
        LabialVelar,
        Velar,
        Uvular,
        Pharyngeal,
        Epiglottal,
        Glottal,
    }

    /// <summary>
    /// Manner of articulation for consonants.
    /// </summary>
    public enum MannerOfArticulation
    {
        Nasal,
        Plosive,
        SibilantFricative,
        NonSibilantFricative,
        Approximant,
        Flap,
        Trill,
        LateralFricative,
        LateralApproximant,
        LateralFlap,
        Click,
        Implosive,
        Ejective
    }

    /// <summary>
    /// Vowel height.
    /// </summary>
    public enum VowelHeight
    {
        Close,
        NearClose,
        CloseMid,
        Mid,
        OpenMid,
        NearOpen,
        Open
    }

    /// <summary>
    /// Horizontal vowel position.
    /// </summary>
    public enum VowelBackness
    {
        Front,
        NearFront,
        Central,
        NearBack,
        Back
    }

    /// <summary>
    /// Vowel roundedness.
    /// </summary>
    public enum VowelRoundedness
    {
        Unrounded,
        LessRounded,
        Rounded,
        MoreRounded
    }

    /// <summary>
    /// A phone is a unit of speech sound.
    /// </summary>
    public struct Phone
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Phone"/> struct.
        /// </summary>
        /// <param name="type">the phone type (consonant or vowel)</param>
        /// <param name="phonation">the phonation (voice intensity)</param>
        /// <param name="place">the place of articulation for consonants.</param>
        /// <param name="manner">the manner of articulation for consonants.</param>
        /// <param name="height">the vowel height.</param>
        /// <param name="backness">the horizontal vowel position.</param>
        /// <param name="roundedness">the vowel roundedness.</param>
        /// <param name="isRhotic">a value indicating whether the phone is rhotic.</param>
        /// <param name="isSyllabic">a value indicating whether the phone is syllabic.</param>
        public Phone(
            PhoneType type, 
            Phonation phonation, 
            PlaceOfArticulation place, 
            MannerOfArticulation manner, 
            VowelHeight height, 
            VowelBackness backness,
            VowelRoundedness roundedness,
            bool isRhotic,
            bool isSyllabic)
        {
            this.Type = type;
            this.Phonation = phonation;
            this.IsSyllabic = isSyllabic;

            if (type == PhoneType.Vowel)
            {
                this.Height = height;
                this.Backness = backness;
                this.Roundedness = roundedness;
                this.IsRhotic = isRhotic;
                this.Place = null;
                this.Manner = null;
            }
            else
            {
                this.Height = null;
                this.Backness = null;
                this.Roundedness = null;
                this.IsRhotic = null;
                this.Place = place;
                this.Manner = manner;
            }
        }

        /// <summary>
        /// Gets the phone type (consonant or vowel)
        /// </summary>
        public PhoneType Type { get; private set; }

        /// <summary>
        /// Gets the phonation (voice intensity)
        /// </summary>
        public Phonation Phonation { get; private set; }

        /// <summary>
        /// Gets the place of articulation for consonants.
        /// </summary>
        public PlaceOfArticulation? Place { get; private set; }

        /// <summary>
        /// Gets the manner of articulation for consonants.
        /// </summary>
        public MannerOfArticulation? Manner { get; private set; }

        /// <summary>
        /// Gets the vowel height.
        /// </summary>
        public VowelHeight? Height { get; private set; }

        /// <summary>
        /// Gets the horizontal vowel position.
        /// </summary>
        public VowelBackness? Backness { get; private set; }

        /// <summary>
        /// Gets the vowel roundedness.
        /// </summary>
        public VowelRoundedness? Roundedness { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the phone is rhotic.
        /// </summary>
        public bool? IsRhotic { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the phone is syllabic.
        /// </summary>
        public bool IsSyllabic { get; private set; }
    }
}
