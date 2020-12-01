namespace TestSLDC
{
    public interface IColorComponent
    {
        /// <summary>
        /// Alpha
        /// </summary>
        byte Alpha { get; }

        /// <summary>
        /// Red
        /// </summary>
        byte Red { get; }

        /// <summary>
        /// Blue
        /// </summary>
        byte Blue { get; }

        /// <summary>
        /// Green
        /// </summary>
        byte Green { get; }

        /// <summary>
        /// Hue
        /// </summary>
        float Hue { get; }

        /// <summary>
        /// Saturation
        /// </summary>
        float Saturation { get; }

        /// <summary>
        /// Brightness
        /// </summary>
        float Brightness { get; }
    }
}
