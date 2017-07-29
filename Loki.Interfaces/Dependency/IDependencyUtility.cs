namespace Loki.Interfaces.Dependency
{
    public interface IDependencyUtility
    {
        void Register<TInterface>(TInterface obj);
        TInterface Resolve<TInterface>();
    }
}