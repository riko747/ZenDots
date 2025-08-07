using Core;
using Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class DefaultModeInstaller : MonoInstaller
    {
        [SerializeField] private DotSpawner dotSpawner;
        [SerializeField] private UIManager uiManager;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<ResourcesManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerPrefsManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<DoTweenManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneLoadManager>().AsSingle();
            Container.BindInstance(uiManager);
            Container.BindInstance(dotSpawner);
        }
    }
}
