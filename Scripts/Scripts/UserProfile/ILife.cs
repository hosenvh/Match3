
using Match3.Foundation.Base.ServiceLocating;

namespace Match3
{

    // NOTE: This code completely violates Command-Query Separation
    public interface ILife : Service
    {
        int Life { get; }
        int MaxLife { get; }

        int GetLife();
        void SetLife(int lifeCount);
        //Return false if Life was equals MaxLife 
        bool IncreaseLife();
        //Return false if Life was equals 0
        bool DecreaseLife();
        void RefillLife();
        //Return false if Life was equals 0
        bool ConsumeAll();
        
        bool IsInInfiniteLife();
        void SetInfiniteLifeSecond(long startTime, int durationInSecond);
        void AddInfiniteLifeSecond(long startTime, int durationInSecond);

        // NOTE: This is commented because no where in the logic it was used.
        //void FinishInfiniteLife();
        //Return time in second
        int GetInInfiniteLifeRemainingTime();
    }
}
