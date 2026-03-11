using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameCore.Mole
{
    public class MoleFieldController : MonoBehaviour
    {
        [SerializeField] private MolePool _molePool;
        [SerializeField] private MoleFieldConstructor _constructor;

        // Время анимации крота в зависимости от сложности
        private const float _easyEmerge = 0.18f, _mediumEmerge = 0.12f, _hardEmerge = 0.08f;
        private const float _easyIdle = 0.7f, _mediumIdle = 0.55f, _hardIdle = 0.4f;
        private const float _easyReturn = 0.14f, _mediumReturn = 0.10f, _hardReturn = 0.07f;

        private const float _easySpawnRate = 1.2f;
        private const float _mediumSpawnRate = 0.9f;
        private const float _hardSpawnRate = 0.6f;

        private Coroutine _spawnRoutine;

        private void Start()
        {
            _constructor.BuildField();
            Core.GameManager.Instance.ShowDifficultyScreen();
        }

        public void OnGameStart()
        {
            if (_spawnRoutine != null)
                StopCoroutine(_spawnRoutine);

            _spawnRoutine = StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                TrySpawnNMoles(3);
                yield return new WaitForSeconds(GetSpawnRate());
            }
        }

        // Спавн сразу N кротов если есть свободные
        private void TrySpawnNMoles(int n)
        {
            var freeMoles = _molePool.GetAllMoles().FindAll(m => m.IsHidden());
            if (freeMoles.Count == 0) return;

            // Перемешиваем список, чтобы выбрать случайные кроты
            ShuffleList(freeMoles);

            int count = Mathf.Min(n, freeMoles.Count);
            (float emerge, float idle, float ret) = GetMoleAnimTimes();
            for (int i = 0; i < count; i++)
            {
                freeMoles[i].Appear(emerge, idle, ret);
            }
        }

        // Utility: перемешать список
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private float GetSpawnRate()
        {
            switch (Core.GameManager.Instance.CurrentDifficulty)
            {
                case Core.GameDifficulty.Easy: return _easySpawnRate;
                case Core.GameDifficulty.Medium: return _mediumSpawnRate;
                case Core.GameDifficulty.Hard: return _hardSpawnRate;
            }
            return _easySpawnRate;
        }

        private (float, float, float) GetMoleAnimTimes()
        {
            switch (Core.GameManager.Instance.CurrentDifficulty)
            {
                case Core.GameDifficulty.Medium: return (_mediumEmerge, _mediumIdle, _mediumReturn);
                case Core.GameDifficulty.Hard: return (_hardEmerge, _hardIdle, _hardReturn);
                default: return (_easyEmerge, _easyIdle, _easyReturn);
            }
        }
    }
}