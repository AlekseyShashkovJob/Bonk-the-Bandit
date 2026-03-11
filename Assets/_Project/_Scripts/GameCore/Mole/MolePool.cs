using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Mole
{
    public class MolePool : MonoBehaviour
    {
        [SerializeField] private Mole _molePrefab;
        [SerializeField] private Transform _parent;
        private readonly List<Mole> _moles = new();

        public void Init(int poolSize)
        {
            if (_moles.Count > 0)
            {
                foreach (var m in _moles) Destroy(m.gameObject);
                _moles.Clear();
            }
            for (int i = 0; i < poolSize; i++)
            {
                Mole mole = Instantiate(_molePrefab, _parent);
                mole.gameObject.SetActive(true);
                _moles.Add(mole);
            }
        }

        public List<Mole> GetAllMoles() => _moles;

        public void ReturnMole(Mole mole)
        {
            // Для UI-решения не деактивируем объект, просто скрываем визуал через HideOnlyMole()
            mole.HideOnlyMole();
        }
    }
}