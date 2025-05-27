using Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class Installer : MonoInstaller
    {
        [SerializeField] private DotSpawner dotSpawner;
        [SerializeField] private UIManager _uiManager;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<ResourcesManager>().AsSingle();
            Container.BindInstance(_uiManager);
            Container.BindInstance(dotSpawner);
        }
    }
}
