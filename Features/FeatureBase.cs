namespace Testplate.Features
{
    public abstract class FeatureBase
    {
        public virtual string Name { get; set; }
        public bool IsEnabled { get; set; }

        public virtual void OnEnable() => IsEnabled = true;
        public virtual void OnDisable() => IsEnabled = false;
        public virtual void OnUpdate() { }
    }
}
