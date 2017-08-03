namespace Loki.Interfaces.Dependency
{
    public interface IDependencyUtility
    {
        /// <summary>
        /// Registers the specified object.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="obj">The object.</param>
        TInterface Register<TInterface>(TInterface obj) where TInterface : class;

        /// <summary>
        /// Resolves the object instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <returns></returns>
        TInterface Resolve<TInterface>();
    }
}