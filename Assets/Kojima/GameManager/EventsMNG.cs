
// script pra unificar os eventos do jogo
public static class EventsMNG
{
    public static event System.Action OnEnemyDeath;
    public static void EnemyDied()
    { OnEnemyDeath?.Invoke(); }
    
    public static event System.Action<int> OnRemainingEnemiesUpdate;
    public static void UpdateEnemiesCounter(int count)
    { OnRemainingEnemiesUpdate?.Invoke(count); }
    
    public static event System.Action<int> OnWaveFinish;
    public static void IncreaseWaveCounter(int wave)
    { OnWaveFinish?.Invoke(wave); }

    public static event System.Action OnWaveStart;
    public static void StartWave()
    { OnWaveStart?.Invoke(); }
}
