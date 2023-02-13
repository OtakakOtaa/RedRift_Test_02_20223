namespace CodeBase.Hand.View
{
    public interface ILayout
    {
        int LayerComponentsCount { get; }
        void SetLayerOrder(int orderAmount);
    }
}