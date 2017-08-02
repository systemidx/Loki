namespace Loki.Interfaces.Dependency
{
    public interface IDependencyUtility
    {
        /// <summary>
        /// Registers the specified object.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="obj">The object.</param>
        void Register<TInterface>(TInterface obj);

        /// <summary>
        /// Resolves the object instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <returns></returns>
        TInterface Resolve<TInterface>();
    }
}