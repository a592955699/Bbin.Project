namespace Bbin.Api.Model
{
    public class CardModel
    {
        public CardModel(string card)
        {
            this.Card = card;
        }
        /// <summary>
        /// 原始结果，例如： D.2
        /// </summary>
        public string Card { get; set; }
        /// <summary>
        /// 牌，例如 2,3,4,5...  11,12,13
        /// </summary>
        public int Result
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Card))
                    return 0;
                else
                {
                    int t = int.Parse(Card.Split(".")[1]);
                    return t >= 10 ? 0 : t;
                }
            }
        }
        /// <summary>
        /// 花色,例如 D H C S
        /// </summary>
        public string Suit
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Card))
                    return "";
                else
                    return Card.Split(".")[0];
            }
        }
    }
}
