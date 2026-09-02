using NAudio.Wave;

namespace rans0m
{
    public class SoundHelper
    {
        /// <summary>
        /// Creates a WaveOut instance from an audio stream.
        /// </summary>
        /// <param name="wavStream">Audio Stream</param>
        /// <returns>Returns a new WaveOut instance initialized with the audio data from the stream</returns>
        public static WaveOut Create(Stream wavStream)
        {
            WaveFileReader reader = new WaveFileReader(wavStream);
            WaveOut waveOut = new WaveOut();

            waveOut.Init(reader);
            return waveOut;
        }

    }
}
