using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameCore.Mole
{
    public class Mole : MonoBehaviour, IPointerClickHandler
    {
        public enum MoleState { Hidden, Emerging, Idle, Hit, Returning }
        public System.Action<Mole> OnMoleHidden;

        [Header("Animation Sprites")]
        [SerializeField] private Sprite[] _emergeSprites;
        [SerializeField] private Sprite[] _hitSprites;
        [SerializeField] private Image _moleImage;

        private const float _hitAnimTime = 0.25f;
        private MoleState _state = MoleState.Hidden;
        private Coroutine _moleRoutine;
        private bool _canBeHit = false;
        private bool _alreadyHit = false;

        public void Appear(float emergeSpeed, float idle, float returnSpeed)
        {
            if (_moleRoutine != null) StopCoroutine(_moleRoutine);
            _alreadyHit = false;
            ShowOnlyMole();

            var rect = _moleImage.rectTransform;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 163f);

            _moleRoutine = StartCoroutine(MoleRoutine(emergeSpeed, idle, returnSpeed));
        }

        public void Hit()
        {
            if (!_canBeHit || _state != MoleState.Idle || _alreadyHit) return;

            _canBeHit = false;
            _alreadyHit = true;
            if (_moleRoutine != null) StopCoroutine(_moleRoutine);
            _moleRoutine = StartCoroutine(HitAnimation());

            Misc.Services.VibroManager.Vibrate();
        }

        public void HideOnlyMole()
        {
            if (_moleImage) _moleImage.enabled = false;
        }

        public void ShowOnlyMole()
        {
            if (_moleImage) _moleImage.enabled = true;
        }

        public bool IsHidden()
        {
            return _state == MoleState.Hidden;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Hit();
        }

        private IEnumerator MoleRoutine(float emergeSpeed, float idle, float returnSpeed)
        {
            _state = MoleState.Emerging;
            _canBeHit = false;
            ShowOnlyMole();

            for (int i = 0; i < 3; i++)
            {
                _moleImage.sprite = _emergeSprites[i];
                yield return new WaitForSeconds(emergeSpeed / 3f);
            }

            _state = MoleState.Idle;
            _canBeHit = true;

            yield return new WaitForSeconds(idle);

            if (!_alreadyHit)
            {
                _state = MoleState.Returning;
                _canBeHit = false;
                for (int i = 3; i < 5; i++)
                {
                    _moleImage.sprite = _emergeSprites[i];
                    yield return new WaitForSeconds(returnSpeed / 2f);
                }
                HideOnlyMole();
                _state = MoleState.Hidden;
                OnMoleHidden?.Invoke(this);
            }
        }

        private IEnumerator HitAnimation()
        {
            _state = MoleState.Hit;
            for (int i = 0; i < _hitSprites.Length; i++)
            {
                _moleImage.sprite = _hitSprites[i];
                yield return new WaitForSeconds(_hitAnimTime / _hitSprites.Length);
            }
            HideOnlyMole();
            _state = MoleState.Hidden;
            OnMoleHidden?.Invoke(this);

            Core.GameManager.Instance.AddScore(1);
        }
    }
}