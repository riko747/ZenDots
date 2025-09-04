using Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private MenuUIManager menuUIManager;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SceneLoadManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerPrefsManager>().AsSingle();
            Container.BindInstance(menuUIManager);
        }
    }
}
