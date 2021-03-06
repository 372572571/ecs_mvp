using MVP.Framework;
using MVP.Framework.Views;

namespace Views
{
    [Link("Coin")]
    [Link("Gold")]

    [Slot("DoBack")]
    [Slot("DoPlay", new object[] {1})]

    public class Home : View
    {
        public string Coin(ulong coin) { return $"C: {coin}"; }
        public string Gold(ulong gold) { return $"G: {gold}"; }
    }
}
