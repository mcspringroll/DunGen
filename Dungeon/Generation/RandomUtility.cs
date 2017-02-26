using System;

namespace DungeonAPI.Generation
{
    /// <summary>
    /// Essentially a wrapper for the Random class
    /// that adds some small utilities like retrieval
    /// of the seed value, and random bools.
    /// </summary>
    public static class RandomUtility
    {
        private const ushort SIZE_LIMIT = 31;

        public static int Seed { get; private set; }

        private static uint nextInt;

        private static ushort bitIndex;

        private static Random valueGenerator;

        /// <summary>
        /// Initializes the random value generator
        /// with the provided seed.
        /// </summary>
        /// <param name="seed"></param>
        public static void Initialize(int seed)
        {
            Seed = seed;
            valueGenerator = new Random(seed);
            UpdateNextInt();
        }

        /// <summary>
        /// Initializes the random value generator
        /// with the system time.
        /// </summary>
        public static void Initialize()
        {
            Initialize((int)(DateTime.UtcNow - DateTime.MinValue).TotalSeconds);
        }

        /// <summary>
        /// Grabs the next int from the generator
        /// and resets byteIndex.
        /// </summary>
        private static void UpdateNextInt()
        {
            nextInt = (uint)valueGenerator.Next();
            bitIndex = 0;
        }

        /// <summary>
        /// Pulls a boolean value from the generator.
        /// </summary>
        /// <returns></returns>
        public static bool RandomBool()
        {
            if (bitIndex == SIZE_LIMIT)
                UpdateNextInt();

            return ((nextInt >> bitIndex++) & 0x01) == 0x01;
        }

        /// <summary>
        /// Pulls a boolean value with the given
        /// probability (should be 0.0 to 1.0).
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static bool RandomBoolWithProbability(double probability)
        {
            return valueGenerator.NextDouble() < probability;
        }

        /// <summary>
        /// Pulls a positive integer value from the
        /// generator.
        /// </summary>
        /// <returns></returns>
        public static int RandomPositiveInt()
        {
            return valueGenerator.Next();
        }
    }
}
