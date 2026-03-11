using UnityEngine;
using View.Button;

namespace View.UI.Menu
{
    public class MainMenuScreen : UIScreen
    {
        [SerializeField] private UIScreen _optionsScreen;
        [SerializeField] private UIScreen _privacyScreen;

        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;

        [Space, Header("Buttons")]
        [SerializeField] private CustomButton _startGame;
        [SerializeField] private CustomButton _settings;
        [SerializeField] private CustomButton _privacy;

        private void OnEnable()
        {
            _startGame.AddListener(OpenGame);
            _settings.AddListener(OpenOptions);
            _privacy.AddListener(OpenPrivacy);
        }

        private void OnDisable()
        {
            _startGame.RemoveListener(OpenGame);
            _settings.RemoveListener(OpenOptions);
            _privacy.RemoveListener(OpenPrivacy);
        }

        public override void StartScreen()
        {
            base.StartScreen();
        }

        private void OpenGame()
        {
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.GAME_SCENE);
            CloseScreen();
        }

        private void OpenOptions()
        {
            _optionsScreen.StartScreen();
        }

        private void OpenPrivacy()
        {
            _privacyScreen.StartScreen();
        }
    }
}