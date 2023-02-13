using System.Collections.Generic;

namespace CodeBase.Hand.View
{
    public class LayerCardSorter
    {
        private const int StartLayerOrder = 0;

        public void SortCards(IEnumerable<ILayout> cards)
        {
            int delta = 0;
            foreach (var card in cards)
            {
                delta+=card.LayerComponentsCount;
                card.SetLayerOrder(StartLayerOrder + card.LayerComponentsCount + delta++);
            }
        }
    }
}