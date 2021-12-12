using UnityEngine;
using UnityEngine.UI;

namespace Sneakers
{
    public class LivesIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject[] livesGos;

        public void SetLives(int livesCount)
        {
            for (int liveIndex = 0; liveIndex < livesGos.Length; liveIndex++)
                livesGos[liveIndex].SetActive(liveIndex < livesCount);
        }
    }
}