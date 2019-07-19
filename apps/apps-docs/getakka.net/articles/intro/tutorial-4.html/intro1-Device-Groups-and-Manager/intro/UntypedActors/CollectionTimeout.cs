namespace intro.UntypedActors
{
    public sealed class CollectionTimeout
    {
        public static CollectionTimeout Instance { get; } = new CollectionTimeout();
        private CollectionTimeout() { }
    }

}