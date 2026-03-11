using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Mole
{
    public class MoleFieldConstructor : MonoBehaviour
    {
        [SerializeField] private RectTransform _fieldParent;
        [SerializeField] private MolePool _molePool;
        [SerializeField] private Sprite _frontHoleSprite;
        [SerializeField] private Sprite _backHoleSprite;

        private readonly Vector2 _spacing = new Vector2(430f, 430f);
        private readonly Vector2Int _gridSize = new Vector2Int(3, 3);

        private readonly float _imageScaleX = 1.25f;
        private readonly float _imageScaleY = 1.25f;

        public void BuildField()
        {
            _molePool.Init(_gridSize.x * _gridSize.y);

            float startX = -(_gridSize.x - 1) * _spacing.x / 2f;
            float startY = -(_gridSize.y - 1) * _spacing.y / 2f;

            Vector2 scale = new Vector2(_imageScaleX, _imageScaleY);

            int idx = 0;
            foreach (var mole in _molePool.GetAllMoles())
            {
                int x = idx % _gridSize.x;
                int y = idx / _gridSize.x;

                Vector2 pos = new Vector2(startX + x * _spacing.x, startY + y * _spacing.y);

                // Root
                GameObject holeRoot = new GameObject($"Hole_{idx}", typeof(RectTransform));
                var rootRect = holeRoot.GetComponent<RectTransform>();
                holeRoot.transform.SetParent(_fieldParent, false);
                rootRect.anchorMin = rootRect.anchorMax = new Vector2(0.5f, 0.5f);
                rootRect.pivot = new Vector2(0.5f, 0.5f);
                rootRect.anchoredPosition = pos;
                rootRect.localScale = Vector3.one;

                // Back hole
                GameObject backGo = new GameObject($"HoleBack_{idx}", typeof(RectTransform), typeof(Image));
                backGo.transform.SetParent(holeRoot.transform, false);
                var backImg = backGo.GetComponent<Image>();
                backImg.sprite = _backHoleSprite;
                backImg.SetNativeSize();
                backImg.raycastTarget = false;

                var backRect = backGo.GetComponent<RectTransform>();
                backRect.anchoredPosition = Vector2.zero;
                backRect.sizeDelta = Vector2.Scale(backRect.sizeDelta, scale);

                // Mole
                mole.transform.SetParent(holeRoot.transform, false);
                var moleRect = mole.GetComponent<RectTransform>();
                moleRect.anchorMin = moleRect.anchorMax = new Vector2(0.5f, 0.5f);
                moleRect.pivot = new Vector2(0.5f, 0.5f);
                moleRect.anchoredPosition = Vector2.zero;
                moleRect.localScale = Vector3.one;

                // Важно: чтобы это работало, у крота должен быть задан размер (обычно через Image.SetNativeSize в его префабе)
                moleRect.sizeDelta = Vector2.Scale(moleRect.sizeDelta, scale);

                // Front hole
                GameObject frontGo = new GameObject($"HoleFront_{idx}", typeof(RectTransform), typeof(Image));
                frontGo.transform.SetParent(holeRoot.transform, false);
                var frontImg = frontGo.GetComponent<Image>();
                frontImg.sprite = _frontHoleSprite;
                frontImg.SetNativeSize();
                frontImg.raycastTarget = false;

                var frontRect = frontGo.GetComponent<RectTransform>();
                frontRect.anchoredPosition = Vector2.zero;
                frontRect.sizeDelta = Vector2.Scale(frontRect.sizeDelta, scale);

                mole.HideOnlyMole();
                mole.OnMoleHidden = (m) => _molePool.ReturnMole(m);

                idx++;
            }
        }
    }
}