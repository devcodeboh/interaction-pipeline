using UnityEngine;

public class GameController : MonoBehaviour
{
    public GamePhase Phase
    {
        get;
        private set;
    }
    private void Awake()
    {
        SetPhase(GamePhase.Boot);
    }
    private void Start()
    {
        SetPhase(GamePhase.Playing);
    }
    private void SetPhase(GamePhase newPhase) => Phase = newPhase;
}
