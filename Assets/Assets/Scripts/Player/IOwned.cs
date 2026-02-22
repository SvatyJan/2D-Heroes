public interface IOwned
{
    Player Owner { get; }
    void SetOwner(Player newOwner);
}