using Assets.CodeBase.Infrastructure.Data;

namespace Assets.CodeBase.Infrastructure.Services.Random
{
    public interface IRandomService : IService
    {
        int Next(int min, int max);
        int ChooseElementFromProbabilityArray(int[] array, int probabilitySum);
        void SetSeed(int seed);
        T Choose<T>(T[] array);
        Direction GetDirection();
        /// <summary>
        /// Return true with given chance, false otherwise
        /// </summary>
        /// <param name="chance"> 0-1</param>
        /// <returns></returns>
        bool Roulette(float chance);

        int GetSeed();
    }
}