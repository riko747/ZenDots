using Managers;
using Zenject;

namespace Installers
{
    public class MainMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SceneLoadManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerPrefsManager>().AsSingle();
        }
    }
}
