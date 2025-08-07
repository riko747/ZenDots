using Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private UIManager uiManager;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SceneLoadManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerPrefsManager>().AsSingle();
            Container.BindInstance(uiManager);
        }
    }
}
