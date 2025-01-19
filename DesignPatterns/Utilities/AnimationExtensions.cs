namespace HIEU_NL.Utilities.Animation
{
    using System;
    using UnityEngine;
    // [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    // public partial class AnimationHashAttribute : PropertyAttribute { }

    public enum ConvertAnimationHashType
    {
        ClipNameToClipHash,
        StateNameToClipHash,
        StateNameToStateHash,
    }

    public class AnimationHashAttribute : PropertyAttribute
    {
        public string                   AnimatorName { get; private set; }
        public int                      Layer        { get; private set; }
        public ConvertAnimationHashType HashType     {get;  private set; }

        public AnimationHashAttribute(ConvertAnimationHashType hashType = ConvertAnimationHashType.StateNameToStateHash, int layer = 0)
        {
            HashType = hashType;
            Layer    = layer;
        }

        public AnimationHashAttribute(string animatorName, ConvertAnimationHashType hashType = ConvertAnimationHashType.StateNameToStateHash, int layer = 0)
        {
            AnimatorName = animatorName;
            HashType     = hashType;
            Layer        = layer;
        }
    }

    public class AnimationParamAttribute : PropertyAttribute
    {
        public string AnimatorName { get; private set; }

        public AnimationParamAttribute()
        {
        }

        public AnimationParamAttribute(string animatorName)
        {
            AnimatorName = animatorName;
        }
    }

    public class AnimationClipLengthAttribute : PropertyAttribute
    {
        public string AnimatorName { get; private set; }

        public AnimationClipLengthAttribute()
        {
        }

        public AnimationClipLengthAttribute(string animatorName)
        {
            AnimatorName = animatorName;
        }
    }

    public class AnimationHashNameAttribute : PropertyAttribute
    {
        public string AnimatorName { get; private set; }

        public AnimationHashNameAttribute()
        {
        }

        public AnimationHashNameAttribute(string animatorName)
        {
            AnimatorName = animatorName;
        }
    }

}