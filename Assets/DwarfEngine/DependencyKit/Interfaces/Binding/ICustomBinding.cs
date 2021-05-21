namespace DwarfEngine.DependencyKit
{
    public interface ICustomBinding
    {
        /// <summary>
        /// Do bindings here. Called before Awake.
        /// </summary>
        void LoadBindings();
    }
}